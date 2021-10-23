using System.Net;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a game hosted in a room which users can join.
    /// </summary>
    internal class Game
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class with the given identification.
        /// </summary>
        /// <param name="id">The unique numerical identifier of the game.</param>
        /// <param name="name">The name of the room being the name of the creator.</param>
        /// <param name="nation">The flag displayed with the game.</param>
        /// <param name="roomID">The ID of the room the game is hosted in.</param>
        /// <param name="ipAddress">The IP address of the host of the game.</param>
        /// <param name="access">The access modifier of the game.</param>
        internal Game(int id, string name, Nation nation, int roomID, IPAddress ipAddress, SessionAccess access)
        {
            ID = id;
            Name = name;
            RoomID = roomID;
            IPAddress = ipAddress;
            Session = new SessionInfo(nation, SessionType.Game, access);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the unique numerical identifier of the game.
        /// </summary>
        internal int ID { get; }

        /// <summary>
        /// Gets the name of the room being the name of the creator.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Gets the ID of the room the game is hosted in.
        /// </summary>
        internal int RoomID { get; }

        /// <summary>
        /// Gets the IP address of the host of the game.
        /// </summary>
        internal IPAddress IPAddress { get; set; }

        /// <summary>
        /// Gets the <see cref="SessionInfo"/> describing the game.
        /// </summary>
        internal SessionInfo Session { get; }
    }
}
