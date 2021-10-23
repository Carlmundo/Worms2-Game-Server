using System.Text;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a unit of communication between Worms 2 client and server.
    /// </summary>
    internal class Packet
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        internal Packet() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class with the given contents.
        /// </summary>
        /// <param name="code">The <see cref="PacketCode"/> describing the action of the packet.</param>
        /// <param name="value0">A parameter for the action.</param>
        /// <param name="value1">A parameter for the action.</param>
        /// <param name="value2">A parameter for the action.</param>
        /// <param name="value3">A parameter for the action.</param>
        /// <param name="value4">A parameter for the action.</param>
        /// <param name="value10">A parameter for the action.</param>
        /// <param name="data">A textual parameter for the action.</param>
        /// <param name="error">An error code returned from the server after executing the action.</param>
        /// <param name="name">A named parameter for the action.</param>
        /// <param name="session">A <see cref="SessionInfo"/> for the action.</param>
        internal Packet(PacketCode code,
            int? value0 = null, int? value1 = null, int? value2 = null, int? value3 = null, int? value4 = null,
            int? value10 = null, string? data = null, int? error = null,
            string? name = null, SessionInfo? session = null)
        {
            Code = code;
            Value0 = value0;
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
            Value10 = value10;
            Data = data;
            Error = error;
            Name = name;
            Session = session;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets <see cref="PacketCode"/> describing the action of the packet.
        /// </summary>
        internal PacketCode Code;

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value0;

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value1;

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value2;

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value3;

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value4;

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value10;

        /// <summary>
        /// Gets or sets a textual parameter for the action.
        /// </summary>
        internal string? Data;

        /// <summary>
        /// Gets or sets an error code returned from the server after executing the action.
        /// </summary>
        internal int? Error;

        /// <summary>
        /// Gets or sets a named parameter for the action.
        /// </summary>
        internal string? Name;

        /// <summary>
        /// Gets or sets a <see cref="SessionInfo"/> for the action.
        /// </summary>
        internal SessionInfo? Session;

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{Code:D} {Code}");
            if (Value0.HasValue) sb.AppendLine($"  {nameof(Value0),7}: {Value0:X8}");
            if (Value1.HasValue) sb.AppendLine($"  {nameof(Value1),7}: {Value1:X8}");
            if (Value2.HasValue) sb.AppendLine($"  {nameof(Value2),7}: {Value2:X8}");
            if (Value3.HasValue) sb.AppendLine($"  {nameof(Value3),7}: {Value3:X8}");
            if (Value4.HasValue) sb.AppendLine($"  {nameof(Value4),7}: {Value4:X8}");
            if (Value10.HasValue) sb.AppendLine($"  {nameof(Value10),7}: {Value10:X8}");
            if (Data != null) sb.AppendLine($"  {nameof(Data),7}: {Data}");
            if (Error.HasValue) sb.AppendLine($"  {nameof(Error),7}: {Error:X8}");
            if (Name != null) sb.AppendLine($"  {nameof(Name),7}: {Name}");
            if (Session.HasValue) sb.AppendLine($"  {nameof(Session),7}: {Session}");
            return sb.ToString().TrimEnd();
        }
    }
}
