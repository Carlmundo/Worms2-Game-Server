using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a duplex connection to a client, allowing to receive and send <see cref="Packet"/> instances.
    /// </summary>
    internal class PacketConnection
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private readonly Stream _stream;
        private readonly object _recvLock = new object();
        private readonly object _sendLock = new object();

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketConnection"/> class communicating with the given
        /// <paramref name="client"/>.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> to communicate with.</param>
        internal PacketConnection(TcpClient client)
        {
            _stream = client.GetStream();
            RemoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the client's <see cref="IPEndPoint"/>.
        /// </summary>
        internal IPEndPoint RemoteEndPoint { get; }

        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Blocks until a <see cref="Packet"/> was received, and returns it.
        /// </summary>
        /// <returns>The received <see cref="Packet"/>.</returns>
        internal Packet Receive()
        {
            lock (_recvLock)
            {
                Packet packet = new Packet();
                packet.Receive(_stream);
                return packet;
            }
        }

        /// <summary>
        /// Blocks until the given <paramref name="packet"/> was sent.
        /// </summary>
        /// <param name="packet">The <see cref="Packet"/> to send.</param>
        internal void Send(Packet packet)
        {
            lock (_sendLock)
                packet.Send(_stream);
        }
    }
}
