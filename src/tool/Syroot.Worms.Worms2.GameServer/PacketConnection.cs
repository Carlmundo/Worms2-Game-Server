using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Syroot.BinaryData.Core;
using Syroot.Worms.Core;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a duplex connection to a client, allowing to receive and send <see cref="Packet"/> instances.
    /// </summary>
    internal sealed class PacketConnection
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _maxDataSize = 0x1000;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private readonly PipeReader _reader;
        private readonly PipeWriter _writer;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketConnection"/> class communicating with the given
        /// <paramref name="client"/>.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> to communicate with.</param>
        internal PacketConnection(TcpClient client)
        {
            Stream stream = client.GetStream();
            _reader = PipeReader.Create(stream);
            _writer = PipeWriter.Create(stream);
            RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint
                ?? throw new ArgumentException("TCP client is not connected.", nameof(client));
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the client's <see cref="IPEndPoint"/>.
        /// </summary>
        internal IPEndPoint RemoteEndPoint { get; }

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Receives a <see cref="Packet"/> instance asynchronously.
        /// </summary>
        /// <returns>The read <see cref="Packet"/> instance.</returns>
        internal async ValueTask<Packet> Read(CancellationToken ct)
        {
            Packet packet = new Packet();
            PacketField at = PacketField.None;
            PacketField fields = PacketField.None;
            int dataLength = 0;
            bool get(in ReadOnlySequence<byte> buffer, out SequencePosition consumedTo)
            {
                consumedTo = default;
                SequenceReader<byte> reader = new SequenceReader<byte>(buffer);
                PacketField prevAt = at;
                switch (at)
                {
                    case PacketField.None:
                        if (!reader.TryReadLittleEndian(out int codeValue)
                            || !reader.TryReadLittleEndian(out int fieldsValue)) break;
                        if (!Enum.IsDefined(typeof(PacketCode), codeValue))
                            throw new InvalidDataException($"Bad packet code {codeValue}.");
                        if (!EnumTools.Validate(typeof(PacketField), fieldsValue))
                            throw new InvalidDataException($"Bad packet fields 0b{Convert.ToString(fieldsValue, 2)}.");
                        packet.Code = (PacketCode)codeValue;
                        fields = (PacketField)fieldsValue;
                        consumedTo = reader.Position;
                        goto case PacketField.Value0;

                    case PacketField.Value0:
                        if (!fields.HasFlag(at = PacketField.Value0)) goto case PacketField.Value1;
                        if (!reader.TryReadLittleEndian(out int value0)) break;
                        packet.Value0 = value0;
                        consumedTo = reader.Position;
                        goto case PacketField.Value1;

                    case PacketField.Value1:
                        if (!fields.HasFlag(at = PacketField.Value1)) goto case PacketField.Value2;
                        if (!reader.TryReadLittleEndian(out int value1)) break;
                        packet.Value1 = value1;
                        consumedTo = reader.Position;
                        goto case PacketField.Value2;

                    case PacketField.Value2:
                        if (!fields.HasFlag(at = PacketField.Value2)) goto case PacketField.Value3;
                        if (!reader.TryReadLittleEndian(out int value2)) break;
                        packet.Value2 = value2;
                        consumedTo = reader.Position;
                        goto case PacketField.Value3;

                    case PacketField.Value3:
                        if (!fields.HasFlag(at = PacketField.Value3)) goto case PacketField.Value4;
                        if (!reader.TryReadLittleEndian(out int value3)) break;
                        packet.Value3 = value3;
                        consumedTo = reader.Position;
                        goto case PacketField.Value4;

                    case PacketField.Value4:
                        if (!fields.HasFlag(at = PacketField.Value4)) goto case PacketField.Value10;
                        if (!reader.TryReadLittleEndian(out int value4)) break;
                        packet.Value4 = value4;
                        consumedTo = reader.Position;
                        goto case PacketField.Value10;

                    case PacketField.Value10:
                        if (!fields.HasFlag(at = PacketField.Value10)) goto case PacketField.DataLength;
                        if (!reader.TryReadLittleEndian(out int value10)) break;
                        packet.Value10 = value10;
                        consumedTo = reader.Position;
                        goto case PacketField.DataLength;

                    case PacketField.DataLength:
                        if (!fields.HasFlag(at = PacketField.DataLength)) goto case PacketField.Error;
                        if (!reader.TryReadLittleEndian(out dataLength)) break;
                        if (dataLength > _maxDataSize)
                            throw new InvalidDataException($"Data too large by {dataLength - _maxDataSize} bytes.");
                        consumedTo = reader.Position;
                        goto case PacketField.Data;

                    case PacketField.Data:
                        if (!fields.HasFlag(at = PacketField.Data)) goto case PacketField.Error;
                        Span<byte> dataBytes = stackalloc byte[dataLength];
                        if (!reader.TryCopyTo(dataBytes)) break;
                        reader.Advance(dataLength);
                        packet.Data = Encodings.Windows1252.GetZeroTerminatedString(dataBytes);
                        consumedTo = reader.Position;
                        goto case PacketField.Error;

                    case PacketField.Error:
                        if (!fields.HasFlag(at = PacketField.Error)) goto case PacketField.Name;
                        if (!reader.TryReadLittleEndian(out int error)) break;
                        packet.Error = error;
                        consumedTo = reader.Position;
                        goto case PacketField.Name;

                    case PacketField.Name:
                        if (!fields.HasFlag(at = PacketField.Name)) goto case PacketField.Session;
                        Span<byte> nameBytes = stackalloc byte[20];
                        if (!reader.TryCopyTo(nameBytes)) break;
                        reader.Advance(20);
                        packet.Name = Encodings.Windows1252.GetZeroTerminatedString(nameBytes);
                        consumedTo = reader.Position;
                        goto case PacketField.Session;

                    case PacketField.Session:
                        if (!fields.HasFlag(at = PacketField.Session)) goto case PacketField.All;
                        Span<byte> sessionBytes = stackalloc byte[Unsafe.SizeOf<SessionInfo>()];
                        if (!reader.TryCopyTo(sessionBytes)) break;
                        reader.Advance(sessionBytes.Length);
                        packet.Session = MemoryMarshal.Cast<byte, SessionInfo>(sessionBytes)[0];
                        consumedTo = reader.Position;
                        goto case PacketField.All;

                    case PacketField.All:
                        at = PacketField.All;
                        break;

                    default:
                        throw new InvalidOperationException("Invalid packet read state.");
                }
                return prevAt < at;
            }

            while (true)
            {
                ReadResult read = await _reader.ReadAsync(ct);
                if (read.IsCanceled)
                    throw new OperationCanceledException("Packet read was canceled.");

                ReadOnlySequence<byte> buffer = read.Buffer;
                if (get(buffer, out SequencePosition consumedTo))
                {
                    _reader.AdvanceTo(consumedTo);
                    if (at == PacketField.All)
                        return packet;
                }

                _reader.AdvanceTo(buffer.Start, buffer.End);
                if (read.IsCompleted)
                    throw new EndOfStreamException("No more data.");
            }
        }

        /// <summary>
        /// Sends a <see cref="Packet"/> instance asynchronously.
        /// </summary>
        /// <param name="packet">The <see cref="Packet"/> instance to write.</param>
        /// <returns>Whether the instance was written successfully.</returns>
        internal async ValueTask<bool> Write(Packet packet, CancellationToken ct)
        {
            unsafe int set()
            {
                // Calculate the (exact) length of the packet.
                PacketField fields = PacketField.None;
                int add(PacketField field, object? value, int size)
                {
                    if (value == null)
                        return 0;
                    fields |= field;
                    return size;
                }
                int size = sizeof(PacketCode) + sizeof(PacketField)
                    + add(PacketField.Value0, packet.Value0, sizeof(int))
                    + add(PacketField.Value1, packet.Value1, sizeof(int))
                    + add(PacketField.Value2, packet.Value2, sizeof(int))
                    + add(PacketField.Value3, packet.Value3, sizeof(int))
                    + add(PacketField.Value4, packet.Value4, sizeof(int))
                    + add(PacketField.Value10, packet.Value10, sizeof(int))
                    + add(PacketField.DataLength, packet.Data, sizeof(int))
                    + add(PacketField.Data, packet.Data, (packet.Data?.Length ?? 0) + 1)
                    + add(PacketField.Error, packet.Error, sizeof(int))
                    + add(PacketField.Name, packet.Name, 20)
                    + add(PacketField.Session, packet.Session, Unsafe.SizeOf<SessionInfo>());

                // Write the data.
                Span<byte> span = _writer.GetSpan(size);
                static void writeInt(ref Span<byte> span, int value)
                {
                    BinaryPrimitives.WriteInt32LittleEndian(span, value);
                    span = span.Slice(sizeof(int));
                }
                writeInt(ref span, (int)packet.Code);
                writeInt(ref span, (int)fields);
                if (packet.Value0 != null) writeInt(ref span, packet.Value0.Value);
                if (packet.Value1 != null) writeInt(ref span, packet.Value1.Value);
                if (packet.Value2 != null) writeInt(ref span, packet.Value2.Value);
                if (packet.Value3 != null) writeInt(ref span, packet.Value3.Value);
                if (packet.Value4 != null) writeInt(ref span, packet.Value4.Value);
                if (packet.Value10 != null) writeInt(ref span, packet.Value10.Value);
                if (packet.Data != null)
                {
                    writeInt(ref span, packet.Data.Length + 1);
                    span = span.Slice(Encodings.Windows1252.GetBytes(packet.Data, span));
                    span[0] = 0;
                    span = span.Slice(1);
                }
                if (packet.Error != null) writeInt(ref span, packet.Error.Value);
                if (packet.Name != null)
                {
                    span[Encodings.Windows1252.GetBytes(packet.Name, span)..20].Clear();
                    span = span.Slice(20);
                }
                if (packet.Session != null)
                {
                    SessionInfo session = packet.Session.Value;
                    new ReadOnlySpan<byte>(Unsafe.AsPointer(ref session), Unsafe.SizeOf<SessionInfo>()).CopyTo(span);
                }

                return size;
            }

            _writer.Advance(set());
            FlushResult flush = await _writer.FlushAsync(ct);
            return !flush.IsCanceled && !flush.IsCompleted;
        }
    }
}
