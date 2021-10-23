using System;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Worms.IO;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a unit of communication between Worms 2 client and server.
    /// </summary>
    internal class Packet
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _maxDataSize = 0x1000;

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
        internal PacketCode Code { get; set; }

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value0 { get; set; }

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value1 { get; set; }

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value2 { get; set; }

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value3 { get; set; }

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value4 { get; set; }

        /// <summary>
        /// Gets or sets a parameter for the action.
        /// </summary>
        internal int? Value10 { get; set; }

        /// <summary>
        /// Gets or sets a textual parameter for the action.
        /// </summary>
        internal string? Data { get; set; }

        /// <summary>
        /// Gets or sets an error code returned from the server after executing the action.
        /// </summary>
        internal int? Error { get; set; }

        /// <summary>
        /// Gets or sets a named parameter for the action.
        /// </summary>
        internal string? Name { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="SessionInfo"/> for the action.
        /// </summary>
        internal SessionInfo? Session { get; set; }

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

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Blocks and reads the packet data from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read the packet data from.</param>
        internal void Receive(Stream stream)
        {
            int dataLength = 0;
            Code = stream.ReadEnum<PacketCode>(true);
            Flags flags = stream.ReadEnum<Flags>(true);
            if (flags.HasFlag(Flags.Value0)) Value0 = stream.ReadInt32();
            if (flags.HasFlag(Flags.Value1)) Value1 = stream.ReadInt32();
            if (flags.HasFlag(Flags.Value2)) Value2 = stream.ReadInt32();
            if (flags.HasFlag(Flags.Value3)) Value3 = stream.ReadInt32();
            if (flags.HasFlag(Flags.Value4)) Value4 = stream.ReadInt32();
            if (flags.HasFlag(Flags.Value10)) Value10 = stream.ReadInt32();
            if (flags.HasFlag(Flags.DataLength)) dataLength = stream.ReadInt32();
            if (flags.HasFlag(Flags.Data) && dataLength >= 0 && dataLength <= _maxDataSize)
                Data = stream.ReadFixedString(dataLength, Encodings.Windows1252);
            if (flags.HasFlag(Flags.Error)) Error = stream.ReadInt32();
            if (flags.HasFlag(Flags.Name)) Name = stream.ReadFixedString(20, Encodings.Windows1252);
            if (flags.HasFlag(Flags.Session)) Session = stream.ReadStruct<SessionInfo>();
        }

        /// <summary>
        /// Blocks and writes the packet data to the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write the packet data to.</param>
        internal void Send(Stream stream)
        {
            stream.WriteEnum(Code);
            stream.WriteEnum(GetFlags());
            if (Value0.HasValue) stream.WriteInt32(Value0.Value);
            if (Value1.HasValue) stream.WriteInt32(Value1.Value);
            if (Value2.HasValue) stream.WriteInt32(Value2.Value);
            if (Value3.HasValue) stream.WriteInt32(Value3.Value);
            if (Value4.HasValue) stream.WriteInt32(Value4.Value);
            if (Value10.HasValue) stream.WriteInt32(Value10.Value);
            if (Data != null)
            {
                stream.WriteInt32(Data.Length + 1);
                stream.WriteFixedString(Data, Data.Length + 1, Encodings.Windows1252);
            }
            if (Error.HasValue) stream.WriteInt32(Error.Value);
            if (Name != null) stream.WriteFixedString(Name, 20, Encodings.Windows1252);
            if (Session.HasValue) stream.WriteStruct(Session.Value);
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private Flags GetFlags()
        {
            Flags flags = Flags.None;
            if (Value0.HasValue) flags |= Flags.Value0;
            if (Value1.HasValue) flags |= Flags.Value1;
            if (Value2.HasValue) flags |= Flags.Value2;
            if (Value3.HasValue) flags |= Flags.Value3;
            if (Value4.HasValue) flags |= Flags.Value4;
            if (Value10.HasValue) flags |= Flags.Value10;
            if (Data != null)
            {
                flags |= Flags.DataLength;
                flags |= Flags.Data;
            }
            if (Error.HasValue) flags |= Flags.Error;
            if (Name != null) flags |= Flags.Name;
            if (Session.HasValue) flags |= Flags.Session;
            return flags;
        }

        // ---- CLASSES, STRUCTS & ENUMS -------------------------------------------------------------------------------

        [Flags]
        private enum Flags : int
        {
            None,
            Value0 = 1 << 0,
            Value1 = 1 << 1,
            Value2 = 1 << 2,
            Value3 = 1 << 3,
            Value4 = 1 << 4,
            Value10 = 1 << 10,
            DataLength = 1 << 5,
            Data = 1 << 6,
            Error = 1 << 7,
            Name = 1 << 8,
            Session = 1 << 9
        }
    }
}
