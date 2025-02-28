using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Syroot.ColoredConsole;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a simplistic game server managing users, rooms, and games.
    /// </summary>
    internal class Server
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _authorizedTimeout = 10 * 60 * 1000;
        private const int _unauthorizedTimeout = 3 * 1000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private int _lastID = 0x1000; // start at an offset to prevent bugs with chat
        private readonly List<User> _users = new List<User>();
        private readonly List<Room> _rooms = new List<Room>();
        private readonly List<Game> _games = new List<Game>();
        private readonly Channel<Func<ValueTask>> _jobs;
        private readonly Dictionary<PacketCode, PacketHandler> _packetHandlers;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        internal Server()
        {
            _jobs = Channel.CreateUnbounded<Func<ValueTask>>(new UnboundedChannelOptions { SingleReader = true });
            _packetHandlers = new Dictionary<PacketCode, PacketHandler>
            {
                [PacketCode.ListRooms] = OnListRooms,
                [PacketCode.ListUsers] = OnListUsers,
                [PacketCode.ListGames] = OnListGames,
                [PacketCode.Login] = OnLogin,
                [PacketCode.CreateRoom] = OnCreateRoom,
                [PacketCode.Join] = OnJoin,
                [PacketCode.Leave] = OnLeave,
                [PacketCode.Close] = OnClose,
                [PacketCode.CreateGame] = OnCreateGame,
                [PacketCode.ChatRoom] = OnChatRoom,
                [PacketCode.ConnectGame] = OnConnectGame,
            };
        }

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Begins listening for new clients connecting to the given <paramref name="localEndPoint"/> and dispatches
        /// them into their own threads.
        /// </summary>
        internal async Task Run(IPEndPoint localEndPoint, CancellationToken ct = default)
        {
            // Begin handling any queued jobs.
            _ = HandleJobs(ct);
            // Begin listening for new connections.
            await HandleConnections(localEndPoint, ct);
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void Log(Color color, string message)
            => ColorConsole.WriteLine(color, $"{DateTime.Now:HH:mm:ss} {message}");

        private static void LogPacket(PacketConnection connection, Packet packet, bool fromClient)
        {
            if (fromClient)
                Log(Color.Cyan, $"{connection.RemoteEndPoint} >> {packet}");
            else
                Log(Color.Magenta, $"{connection.RemoteEndPoint} << {packet}");
        }

        private static async ValueTask SendPacket(PacketConnection connection, CancellationToken ct, Packet packet)
        {
            LogPacket(connection, packet, false);
            await connection.Write(packet, ct);
        }

        private User? GetUser(PacketConnection connection)
        {
            foreach (User user in _users)
                if (user.Connection == connection)
                    return user;
            return null;
        }

        private async ValueTask HandleJobs(CancellationToken ct)
        {
            await foreach (Func<ValueTask> job in _jobs.Reader.ReadAllAsync(ct))
                await job();
        }

        private async ValueTask HandleConnections(IPEndPoint localEndPoint, CancellationToken ct)
        {
            // Start a new listener for new incoming connections.
            TcpListener listener = new TcpListener(localEndPoint);
            listener.Start();
            Log(Color.Orange, $"Server listening under {listener.LocalEndpoint}...");

            // Dispatch each connection into its own thread.
            TcpClient? client;
            while ((client = await listener.AcceptTcpClientAsync(ct)) != null)
                _ = HandleClient(client, ct);
        }

        private async Task HandleClient(TcpClient client, CancellationToken ct)
        {
            PacketConnection connection = new PacketConnection(client);
            Log(Color.Green, $"{connection.RemoteEndPoint} connected.");
            bool loggedIn = false;

            try
            {
                while (true)
                {
                    // Receive packet during a valid time frame.
                    CancellationTokenSource timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    timeoutCts.CancelAfter(loggedIn ? _authorizedTimeout : _unauthorizedTimeout);
                    Packet packet = await connection.Read(timeoutCts.Token);

                    // Log packet.
                    LogPacket(connection, packet, true);
                    if (packet.Code == PacketCode.Login)
                        loggedIn = true;

                    // Queue handling of known queries.
                    if (_packetHandlers.TryGetValue(packet.Code, out PacketHandler? handler))
                        await _jobs.Writer.WriteAsync(() => handler(connection, packet, ct), ct);
                    else
                        Log(Color.Red, $"{connection.RemoteEndPoint} unhandled {packet.Code}.");
                }
            }
            catch (Exception ex)
            {
                Log(Color.Red, $"{connection.RemoteEndPoint} disconnected. {ex.Message}");

                ct.ThrowIfCancellationRequested();
                await _jobs.Writer.WriteAsync(() => OnDisconnectUserAsync(connection, ct), ct);
            }
        }

        private async ValueTask LeaveRoom(Room? room, int leftID, CancellationToken ct)
        {
            // Close an abandoned room.
            bool roomClosed = room != null
                && !_users.Any(x => x.ID != leftID && x.RoomID == room.ID)
                && !_games.Any(x => x.ID != leftID && x.RoomID == room.ID);
            if (roomClosed)
                _rooms.Remove(room!);

            // Notify other users.
            foreach (User user in _users.Where(x => x.ID != leftID))
            {
                // Notify room leave, if any.
                if (room != null)
                {
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.Leave,
                        value2: room.ID,
                        value10: leftID));
                }
                // Notify room close, if any.
                if (roomClosed)
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.Close, value10: room!.ID));
            }
        }

        // ---- Handlers ----

        private async ValueTask OnListRooms(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value4 != 0)
                return;

            foreach (Room room in _rooms)
            {
                await SendPacket(connection, ct, new Packet(PacketCode.ListItem,
                    value1: room.ID,
                    data: String.Empty, // do not report creator IP
                    name: room.Name,
                    session: room.Session));
            }
            await SendPacket(connection, ct, new Packet(PacketCode.ListEnd));
        }

        private async ValueTask OnListUsers(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value2 != fromUser.RoomID || packet.Value4 != 0)
                return;

            foreach (User user in _users.Where(x => x.RoomID == fromUser.RoomID)) // notably includes the user itself
            {
                await SendPacket(connection, ct, new Packet(PacketCode.ListItem,
                    value1: user.ID,
                    name: user.Name,
                    session: user.Session));
            }
            await SendPacket(connection, ct, new Packet(PacketCode.ListEnd));
        }

        private async ValueTask OnListGames(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value2 != fromUser.RoomID || packet.Value4 != 0)
                return;

            foreach (Game game in _games.Where(x => x.RoomID == fromUser.RoomID))
            {
                await SendPacket(connection, ct, new Packet(PacketCode.ListItem,
                    value1: game.ID,
                    data: game.GameID,
                    name: game.Name,
                    session: game.Session));
            }
            await SendPacket(connection, ct, new Packet(PacketCode.ListEnd));
        }

        private async ValueTask OnLogin(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            if (packet.Value1 == null || packet.Value4 == null || packet.Name == null || packet.Session == null)
                return;

            // Check if user name is valid and not already taken.
            if (_users.Any(x => x.Name.Equals(packet.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                await SendPacket(connection, ct, new Packet(PacketCode.LoginReply, value1: 0, error: 1));
            }
            else
            {
                User newUser = new User(connection, ++_lastID, packet.Name, packet.Session.Value.Nation);

                // Notify other users about new user.
                foreach (User user in _users)
                {
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.Login,
                        value1: newUser.ID, value4: 0, name: newUser.Name, session: newUser.Session));
                }

                // Register new user and send reply to him.
                _users.Add(newUser);
                await SendPacket(connection, ct, new Packet(PacketCode.LoginReply, value1: newUser.ID, error: 0));
            }
        }

        private async ValueTask OnCreateRoom(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value1 != 0 || packet.Value4 != 0 || packet.Data == null
                || packet.Name == null || packet.Session == null)
                return;

            // Check if room name is valid is not already taken.
            if (!_rooms.Any(x => x.Name.Equals(packet.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                Room newRoom = new Room(++_lastID, packet.Name, packet.Session.Value.Nation,
                    connection.RemoteEndPoint.Address);
                _rooms.Add(newRoom);

                // Notify other users about new room.
                foreach (User user in _users.Where(x => x != fromUser))
                {
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.CreateRoom,
                        value1: newRoom.ID,
                        value4: 0,
                        data: String.Empty, // do not report creator IP
                        name: newRoom.Name,
                        session: newRoom.Session));
                }

                // Send reply to creator.
                await SendPacket(connection, ct, new Packet(PacketCode.CreateRoomReply, value1: newRoom.ID, error: 0));
            }
            else
            {
                await SendPacket(connection, ct, new Packet(PacketCode.CreateRoomReply, value1: 0, error: 1));
            }
        }

        private async ValueTask OnJoin(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value2 == null || packet.Value10 != fromUser.ID)
                return;

            // Require valid room or game ID.
            if (_rooms.Any(x => x.ID == packet.Value2))
            {
                fromUser.RoomID = packet.Value2.Value;

                // Notify other users about the join.
                foreach (User user in _users.Where(x => x != fromUser))
                {
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.Join,
                        value2: fromUser.RoomID,
                        value10: fromUser.ID));
                }

                // Send reply to joiner.
                await SendPacket(connection, ct, new Packet(PacketCode.JoinReply, error: 0));
            }
            else if (_games.Any(x => x.ID == packet.Value2 && x.RoomID == fromUser.RoomID))
            {
                // Notify other users about the join.
                foreach (User user in _users.Where(x => x != fromUser))
                {
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.Join,
                        value2: fromUser.RoomID,
                        value10: fromUser.ID));
                }

                // Send reply to joiner.
                await SendPacket(connection, ct, new Packet(PacketCode.JoinReply, error: 0));
            }
            else
            {
                await SendPacket(connection, ct, new Packet(PacketCode.JoinReply, error: 1));
            }
        }

        private async ValueTask OnLeave(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value2 == null || packet.Value10 != fromUser.ID)
                return;

            // Require valid room ID (never sent for games, users disconnect if leaving a game).
            if (packet.Value2 == fromUser.RoomID)
            {
                await LeaveRoom(_rooms.FirstOrDefault(x => x.ID == fromUser.RoomID), fromUser.ID, ct);
                fromUser.RoomID = 0;

                // Reply to leaver.
                await SendPacket(connection, ct, new Packet(PacketCode.LeaveReply, error: 0));
            }
            else
            {
                // Reply to leaver.
                await SendPacket(connection, ct, new Packet(PacketCode.LeaveReply, error: 1));
            }
        }

        private async ValueTask OnDisconnectUserAsync(PacketConnection connection, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null)
                return;

            int roomID = fromUser.RoomID;
            int leftID = fromUser.ID;
            _users.Remove(fromUser);

            // Close an abandoned game.
            Game? game = _games.SingleOrDefault(x => x.Name == fromUser.Name);
            if (game != null)
            {
                roomID = game.RoomID;
                leftID = game.ID;
                _games.Remove(game);
                // Notify other users.
                foreach (User user in _users.Where(x => x != fromUser))
                {
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.Leave,
                        value2: game.ID, value10: fromUser.ID));
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.Close,
                        value10: game.ID));
                }
            }

            // Close any abandoned room.
            await LeaveRoom(_rooms.FirstOrDefault(x => x.ID == roomID), leftID, ct);

            // Notify user disconnect.
            foreach (User user in _users)
                await SendPacket(user.Connection, ct, new Packet(PacketCode.DisconnectUser, value10: fromUser.ID));
        }

        private async ValueTask OnClose(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value10 == null)
                return;

            // Never sent for games, users disconnect if leaving a game.
            // Simply reply success to client, the server decides when to actually close rooms.
            await SendPacket(connection, ct, new Packet(PacketCode.CloseReply, error: 0));
        }

        private async ValueTask OnCreateGame(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value1 != 0 || packet.Value2 != fromUser.RoomID || packet.Value4 != 0x800
                || packet.Data == null || packet.Name == null || packet.Session == null)
                return;

            // Require valid room ID and game ID.
            if(!string.IsNullOrWhiteSpace(packet.Data))
            {
                Game newGame = new Game(++_lastID, fromUser.Name, fromUser.Session.Nation, fromUser.RoomID,
                    packet.Data, // do not use bad NAT IP reported by users here
                    packet.Session.Value.Access);
                _games.Add(newGame);

                // Notify other users about new game, even those in other rooms.
                foreach (User user in _users.Where(x => x != fromUser))
                {
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.CreateGame,
                        value1: newGame.ID,
                        value2: newGame.RoomID,
                        value4: 0x800,
                        data: newGame.GameID,
                        name: newGame.Name,
                        session: newGame.Session));
                }

                // Send reply to host.
                await SendPacket(connection, ct, new Packet(PacketCode.CreateGameReply, value1: newGame.ID, error: 0));
            }
            else
            {
                await SendPacket(connection, ct, new Packet(PacketCode.CreateGameReply, value1: 0, error: 2));
                await SendPacket(connection, ct, new Packet(PacketCode.ChatRoom,
                    value0: fromUser.ID,
                    value3: fromUser.RoomID,
                    data: $"GRP:Cannot host your game."));
            }
        }

        private async ValueTask OnChatRoom(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value0 != fromUser.ID || packet.Value3 == null || packet.Data == null)
                return;

            int targetID = packet.Value3.Value;
            string prefix;
            if (packet.Data.StartsWith(prefix = $"GRP:[ {fromUser.Name} ]  ", StringComparison.InvariantCulture))
            {
                // Check if user can access the room.
                if (fromUser.RoomID == targetID)
                {
                    // Notify all users of the room.
                    string message = packet.Data.Substring(prefix.Length);
                    foreach (User user in _users.Where(x => x.RoomID == fromUser.RoomID && x != fromUser))
                    {
                        await SendPacket(user.Connection, ct, new Packet(PacketCode.ChatRoom,
                            value0: fromUser.ID,
                            value3: user.RoomID,
                            data: prefix + message));
                    }
                    // Notify sender.
                    await SendPacket(connection, ct, new Packet(PacketCode.ChatRoomReply, error: 0));
                }
                else
                {
                    await SendPacket(connection, ct, new Packet(PacketCode.ChatRoomReply, error: 1));
                }
            }
            else if (packet.Data.StartsWith(prefix = $"PRV:[ {fromUser.Name} ]  ", StringComparison.InvariantCulture))
            {
                // Check if user can access the user.
                User? user = _users.FirstOrDefault(x => x.RoomID == fromUser.RoomID && x.ID == targetID);
                if (user == null)
                {
                    await SendPacket(connection, ct, new Packet(PacketCode.ChatRoomReply, error: 1));
                }
                else
                {
                    // Notify receiver of the message.
                    string message = packet.Data.Substring(prefix.Length);
                    await SendPacket(user.Connection, ct, new Packet(PacketCode.ChatRoom,
                        value0: fromUser.ID,
                        value3: user.ID,
                        data: prefix + message));
                    // Notify sender.
                    await SendPacket(connection, ct, new Packet(PacketCode.ChatRoomReply, error: 0));
                }
            }
        }

        private async ValueTask OnConnectGame(PacketConnection connection, Packet packet, CancellationToken ct)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value0 == null)
                return;

            // Require valid game ID and user to be in appropriate room.
            Game? game = _games.FirstOrDefault(x => x.ID == packet.Value0 && x.RoomID == fromUser.RoomID);
            if (game == null)
            {
                await SendPacket(connection, ct, new Packet(PacketCode.ConnectGameReply,
                    data: String.Empty,
                    error: 1));
            }
            else
            {
                await SendPacket(connection, ct, new Packet(PacketCode.ConnectGameReply,
                    data: game.GameID,
                    error: 0));
            }
        }

        // ---- CLASSES, STRUCTS & ENUMS -------------------------------------------------------------------------------

        private delegate ValueTask PacketHandler(PacketConnection connection, Packet packet, CancellationToken ct);
    }
}
