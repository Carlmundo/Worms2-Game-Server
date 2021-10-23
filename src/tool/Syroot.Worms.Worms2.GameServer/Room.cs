using System.Net;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a room in which users can meet, chat, and host games.
    /// </summary>
    internal class Room
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class with the given identificatoin.
        /// </summary>
        /// <param name="id">The unique numerical identifier of the room.</param>
        /// <param name="name">The name of the room as given by the creator.</param>
        /// <param name="nation">The flag displayed with the room.</param>
        /// <param name="ipAddress">The IP address of the creator of the room.</param>
        internal Room(int id, string name, Nation nation, IPAddress ipAddress)
        {
            ID = id;
            Name = name;
            Session = new SessionInfo(nation, SessionType.Room);
            IPAddress = ipAddress;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the unique numerical identifier of the room.
        /// </summary>
        internal int ID { get; }

        /// <summary>
        /// Gets the name of the room as given by the creator.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Gets the <see cref="SessionInfo"/> describing the room.
        /// </summary>
        internal SessionInfo Session { get; }

        /// <summary>
        /// Gets the IP address of the creator of the room.
        /// </summary>
        internal IPAddress IPAddress { get; set; }
    }
}
