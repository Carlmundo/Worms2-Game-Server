namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents information on a client connected to the server.
    /// </summary>
    internal class User
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class with the given identification.
        /// </summary>
        /// <param name="connection">The <see cref="PacketCode"/> to communicate through.</param>
        /// <param name="id">The unique numerical identifier of the user.</param>
        /// <param name="name">The textual login name displayed to others.</param>
        /// <param name="nation">The flag displayed to others.</param>
        internal User(PacketConnection connection, int id, string name, Nation nation)
        {
            Connection = connection;
            ID = id;
            Name = name;
            Session = new SessionInfo(nation, SessionType.User);
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the <see cref="PacketConnection"/> to communicate through.
        /// </summary>
        internal PacketConnection Connection { get; }

        /// <summary>
        /// Gets the unique numerical identifier of the user.
        /// </summary>
        internal int ID { get; }

        /// <summary>
        /// Gets the textual login name displayed to others.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Gets the <see cref="SessionInfo"/> describing the user.
        /// </summary>
        internal SessionInfo Session { get; }

        /// <summary>
        /// Gets or sets the ID of the <see cref="Room"/> the user is in, or 0 for no room.
        /// </summary>
        internal int RoomID { get; set; }
    }
}
