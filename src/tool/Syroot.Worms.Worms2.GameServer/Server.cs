using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Syroot.ColoredConsole;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a simplistic game server managing users, rooms, and games.
    /// </summary>
    internal class Server
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private int _lastID = 0x1000; // start at an offset to prevent bugs with chat
        private readonly List<User> _users = new List<User>();
        private readonly List<Room> _rooms = new List<Room>();
        private readonly List<Game> _games = new List<Game>();
        private readonly BlockingCollection<Action> _jobs = new BlockingCollection<Action>();
        private readonly Dictionary<PacketCode, Action<PacketConnection, Packet>> _packetHandlers;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        internal Server() => _packetHandlers = new Dictionary<PacketCode, Action<PacketConnection, Packet>>
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

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Begins listening for new clients connecting to the given <paramref name="localEndPoint"/> and dispatches
        /// them into their own threads.
        /// </summary>
        internal void Run(IPEndPoint localEndPoint)
        {
            // Begin handling any queued jobs.
            Task.Run(() => HandleJobs());
            // Begin listening for new connections. Currently synchronous and blocking.
            HandleConnections(localEndPoint);
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void SendPacket(PacketConnection connection, Packet packet)
        {
            LogPacket(connection, packet, false);
            connection.Send(packet);
        }

        private static void LogPacket(PacketConnection connection, Packet packet, bool fromClient)
        {
            if (fromClient)
                ColorConsole.WriteLine(Color.Cyan, $"{DateTime.Now:HH:mm:ss} {connection.RemoteEndPoint} >> {packet}");
            else
                ColorConsole.WriteLine(Color.Magenta, $"{DateTime.Now:HH:mm:ss} {connection.RemoteEndPoint} << {packet}");
        }

        private User? GetUser(PacketConnection connection)
        {
            foreach (User user in _users)
                if (user.Connection == connection)
                    return user;
            return null;
        }

        private void HandleJobs()
        {
            foreach (Action job in _jobs.GetConsumingEnumerable())
                job();
        }

        private void HandleConnections(IPEndPoint localEndPoint)
        {
            // Start a new listener for new incoming connections.
            TcpListener listener = new TcpListener(localEndPoint);
            listener.Start();
            ColorConsole.WriteLine(Color.Orange, $"Server listening under {listener.LocalEndpoint}...");

            // Dispatch each connection into its own thread.
            TcpClient? client;
            while ((client = listener.AcceptTcpClient()) != null)
                Task.Run(() => HandleClient(client));
        }

        private void HandleClient(TcpClient client)
        {
            PacketConnection connection = new PacketConnection(client);
            ColorConsole.WriteLine(Color.Green, $"{connection.RemoteEndPoint} connected.");

            try
            {
                while (true)
                {
                    // Receive and log query.
                    Packet packet = connection.Receive();
                    LogPacket(connection, packet, true);

                    // Queue handling of known queries.
                    if (_packetHandlers.TryGetValue(packet.Code, out Action<PacketConnection, Packet>? handler))
                        _jobs.Add(() => handler(connection, packet));
                    else
                        ColorConsole.WriteLine(Color.Red, $"{connection.RemoteEndPoint} unhandled {packet.Code}.");
                }
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(Color.Red, $"{connection.RemoteEndPoint} disconnected. {ex.Message}");
                _jobs.Add(() => OnDisconnectUser(connection));
            }
        }

        private void LeaveRoom(Room? room, int leftID)
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
                    SendPacket(user.Connection, new Packet(PacketCode.Leave,
                        value2: room.ID,
                        value10: leftID));
                }
                // Notify room close, if any.
                if (roomClosed)
                    SendPacket(user.Connection, new Packet(PacketCode.Close, value10: room!.ID));
            }
        }

        // ---- Handlers ----

        private void OnListRooms(PacketConnection connection, Packet packet)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value4 != 0)
                return;

            foreach (Room room in _rooms)
            {
                SendPacket(connection, new Packet(PacketCode.ListItem,
                    value1: room.ID,
                    data: String.Empty, // do not report creator IP
                    name: room.Name,
                    session: room.Session));
            }
            SendPacket(connection, new Packet(PacketCode.ListEnd));
        }

        private void OnListUsers(PacketConnection connection, Packet packet)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value2 != fromUser.RoomID || packet.Value4 != 0)
                return;

            foreach (User user in _users.Where(x => x.RoomID == fromUser.RoomID)) // notably includes the user itself
            {
                SendPacket(connection, new Packet(PacketCode.ListItem,
                    value1: user.ID,
                    name: user.Name,
                    session: user.Session));
            }
            SendPacket(connection, new Packet(PacketCode.ListEnd));
        }

        private void OnListGames(PacketConnection connection, Packet packet)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value2 != fromUser.RoomID || packet.Value4 != 0)
                return;

            foreach (Game game in _games.Where(x => x.RoomID == fromUser.RoomID))
            {
                SendPacket(connection, new Packet(PacketCode.ListItem,
                    value1: game.ID,
                    data: game.IPAddress.ToString(),
                    name: game.Name,
                    session: game.Session));
            }
            SendPacket(connection, new Packet(PacketCode.ListEnd));
        }

        private void OnLogin(PacketConnection connection, Packet packet)
        {
            if (packet.Value1 == null || packet.Value4 == null || packet.Name == null || packet.Session == null)
                return;

            // Check if user name is valid and not already taken.
            if (_users.Any(x => x.Name.Equals(packet.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                SendPacket(connection, new Packet(PacketCode.LoginReply, value1: 0, error: 1));
            }
            else
            {
                User newUser = new User(connection, ++_lastID, packet.Name, packet.Session.Value.Nation);

                // Notify other users about new user.
                foreach (User user in _users)
                {
                    SendPacket(user.Connection, new Packet(PacketCode.Login,
                        value1: newUser.ID,
                        value4: 0,
                        name: newUser.Name,
                        session: newUser.Session));
                }

                // Register new user and send reply to him.
                _users.Add(newUser);
                SendPacket(connection, new Packet(PacketCode.LoginReply, value1: newUser.ID, error: 0));
            }
        }

        private void OnCreateRoom(PacketConnection connection, Packet packet)
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
                    SendPacket(user.Connection, new Packet(PacketCode.CreateRoom,
                        value1: newRoom.ID,
                        value4: 0,
                        data: String.Empty, // do not report creator IP
                        name: newRoom.Name,
                        session: newRoom.Session));
                }

                // Send reply to creator.
                SendPacket(connection, new Packet(PacketCode.CreateRoomReply,
                    value1: newRoom.ID,
                    error: 0));
            }
            else
            {
                SendPacket(connection, new Packet(PacketCode.CreateRoomReply,
                    value1: 0,
                    error: 1));
            }
        }

        private void OnJoin(PacketConnection connection, Packet packet)
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
                    SendPacket(user.Connection, new Packet(PacketCode.Join,
                        value2: fromUser.RoomID,
                        value10: fromUser.ID));
                }

                // Send reply to joiner.
                SendPacket(connection, new Packet(PacketCode.JoinReply, error: 0));
            }
            else if (_games.Any(x => x.ID == packet.Value2 && x.RoomID == fromUser.RoomID))
            {
                // Notify other users about the join.
                foreach (User user in _users.Where(x => x != fromUser))
                {
                    SendPacket(user.Connection, new Packet(PacketCode.Join,
                        value2: fromUser.RoomID,
                        value10: fromUser.ID));
                }

                // Send reply to joiner.
                SendPacket(connection, new Packet(PacketCode.JoinReply, error: 0));
            }
            else
            {
                SendPacket(connection, new Packet(PacketCode.JoinReply, error: 1));
            }
        }

        private void OnLeave(PacketConnection connection, Packet packet)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value2 == null || packet.Value10 != fromUser.ID)
                return;

            // Require valid room ID (never sent for games, users disconnect if leaving a game).
            if (packet.Value2 == fromUser.RoomID)
            {
                LeaveRoom(_rooms.FirstOrDefault(x => x.ID == fromUser.RoomID), fromUser.ID);
                fromUser.RoomID = 0;

                // Reply to leaver.
                SendPacket(connection, new Packet(PacketCode.LeaveReply, error: 0));
            }
            else
            {
                // Reply to leaver.
                SendPacket(connection, new Packet(PacketCode.LeaveReply, error: 1));
            }
        }

        private void OnDisconnectUser(PacketConnection connection)
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
                    SendPacket(user.Connection, new Packet(PacketCode.Leave, value2: game.ID, value10: fromUser.ID));
                    SendPacket(user.Connection, new Packet(PacketCode.Close, value10: game.ID));
                }
            }

            // Close any abandoned room.
            LeaveRoom(_rooms.FirstOrDefault(x => x.ID == roomID), leftID);

            // Notify user disconnect.
            foreach (User user in _users)
            {
                SendPacket(user.Connection, new Packet(PacketCode.DisconnectUser,
                    value10: fromUser.ID));
            }
        }

        private void OnClose(PacketConnection connection, Packet packet)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value10 == null)
                return;

            // Never sent for games, users disconnect if leaving a game.
            // Simply reply success to client, the server decides when to actually close rooms.
            SendPacket(connection, new Packet(PacketCode.CloseReply, error: 0));
        }

        private void OnCreateGame(PacketConnection connection, Packet packet)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value1 != 0 || packet.Value2 != fromUser.RoomID || packet.Value4 != 0x800
                || packet.Data == null || packet.Name == null || packet.Session == null)
                return;

            // Require valid room ID and IP.
            if (IPAddress.TryParse(packet.Data, out IPAddress ip) && connection.RemoteEndPoint.Address.Equals(ip))
            {
                Game newGame = new Game(++_lastID, fromUser.Name, fromUser.Session.Nation, fromUser.RoomID,
                    connection.RemoteEndPoint.Address, // do not use bad NAT IP reported by users here
                    packet.Session.Value.Access);
                _games.Add(newGame);

                // Notify other users about new game, even those in other rooms.
                foreach (User user in _users.Where(x => x != fromUser))
                {
                    SendPacket(user.Connection, new Packet(PacketCode.CreateGame,
                        value1: newGame.ID,
                        value2: newGame.RoomID,
                        value4: 0x800,
                        data: newGame.IPAddress.ToString(),
                        name: newGame.Name,
                        session: newGame.Session));
                }

                // Send reply to host.
                SendPacket(connection, new Packet(PacketCode.CreateGameReply, value1: newGame.ID, error: 0));
            }
            else
            {
                SendPacket(connection, new Packet(PacketCode.CreateGameReply, value1: 0, error: 2));
                SendPacket(connection, new Packet(PacketCode.ChatRoom,
                    value0: fromUser.ID,
                    value3: fromUser.RoomID,
                    data: $"GRP:Cannot host your game. Please use the Worms 2 Memory Changer to set your IP "
                        + $"{fromUser.Connection.RemoteEndPoint.Address}. For more information, visit "
                        + "worms2d.info/Worms_2_Memory_Changer"));
            }
        }

        private void OnChatRoom(PacketConnection connection, Packet packet)
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
                        SendPacket(user.Connection, new Packet(PacketCode.ChatRoom,
                            value0: fromUser.ID,
                            value3: user.RoomID,
                            data: prefix + message));
                    }
                    // Notify sender.
                    SendPacket(connection, new Packet(PacketCode.ChatRoomReply, error: 0));
                }
                else
                {
                    SendPacket(connection, new Packet(PacketCode.ChatRoomReply, error: 1));
                }
            }
            else if (packet.Data.StartsWith(prefix = $"PRV:[ {fromUser.Name} ]  ", StringComparison.InvariantCulture))
            {
                // Check if user can access the user.
                User? user = _users.FirstOrDefault(x => x.RoomID == fromUser.RoomID && x.ID == targetID);
                if (user == null)
                {
                    SendPacket(connection, new Packet(PacketCode.ChatRoomReply, error: 1));
                }
                else
                {
                    // Notify receiver of the message.
                    string message = packet.Data.Substring(prefix.Length);
                    SendPacket(user.Connection, new Packet(PacketCode.ChatRoom,
                        value0: fromUser.ID,
                        value3: user.ID,
                        data: prefix + message));
                    // Notify sender.
                    SendPacket(connection, new Packet(PacketCode.ChatRoomReply, error: 0));
                }
            }
        }

        private void OnConnectGame(PacketConnection connection, Packet packet)
        {
            User? fromUser = GetUser(connection);
            if (fromUser == null || packet.Value0 == null)
                return;

            // Require valid game ID and user to be in appropriate room.
            Game? game = _games.FirstOrDefault(x => x.ID == packet.Value0 && x.RoomID == fromUser.RoomID);
            if (game == null)
            {
                SendPacket(connection, new Packet(PacketCode.ConnectGameReply,
                    data: String.Empty,
                    error: 1));
            }
            else
            {
                SendPacket(connection, new Packet(PacketCode.ConnectGameReply,
                    data: game.IPAddress.ToString(),
                    error: 0));
            }
        }
    }
}
