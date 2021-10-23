namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents the description of the packet contents, as seen from client-side (thus a "reply" comes from the
    /// server).
    /// </summary>
    internal enum PacketCode : int
    {
        ListRooms = 200,
        ListItem = 350,
        ListEnd = 351,
        ListUsers = 400,
        ListGames = 500,
        Login = 600,
        LoginReply = 601,
        CreateRoom = 700,
        CreateRoomReply = 701,
        Join = 800,
        JoinReply = 801,
        Leave = 900,
        LeaveReply = 901,
        DisconnectUser = 1000,
        Close = 1100,
        CloseReply = 1101,
        CreateGame = 1200,
        CreateGameReply = 1201,
        ChatRoom = 1300,
        ChatRoomReply = 1301,
        ConnectGame = 1326,
        ConnectGameReply = 1327,
    }
}
