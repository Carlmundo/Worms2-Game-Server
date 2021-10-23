using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Syroot.ColoredConsole;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a proxy dumping Worms 2 network traffic to console for debug purposes.
    /// </summary>
    internal static class Proxy
    {
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        internal static void Run(IPEndPoint localEndPoint)
        {
            // Start listening for clients to intercept.
            TcpListener listener = new TcpListener(localEndPoint);
            listener.Start();
            ColorConsole.WriteLine(Color.Orange, $"Proxy listening under {localEndPoint}...");

            TcpClient? client;
            while ((client = listener.AcceptTcpClient()) != null)
            {
                // Connect to server.
                TcpClient server = new TcpClient();
                server.Connect("uk1.servers.worms2.com", 17171);

                PacketConnection clientConnection = new PacketConnection(client);
                PacketConnection serverConnection = new PacketConnection(server);
                ColorConsole.WriteLine(Color.Green, $"{clientConnection.RemoteEndPoint} connected.");

                Task.Run(() => Forward(clientConnection, serverConnection, true));
                Task.Run(() => Forward(serverConnection, clientConnection, false));
            }
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void Forward(PacketConnection from, PacketConnection to, bool fromClient)
        {
            while (true)
            {
                Packet packet = from.Receive();
                if (fromClient)
                    ColorConsole.WriteLine(Color.Cyan, $"{from.RemoteEndPoint} >> {to.RemoteEndPoint} | {packet}");
                else
                    ColorConsole.WriteLine(Color.Magenta, $"{to.RemoteEndPoint} << {from.RemoteEndPoint} | {packet}");
                to.Send(packet);
            }
        }
    }
}
