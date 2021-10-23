using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

        internal static async Task Run(IPEndPoint localEndPoint, CancellationToken ct = default)
        {
            // Start listening for clients to intercept.
            TcpListener listener = new TcpListener(localEndPoint);
            listener.Start();
            Log(Color.Orange, $"Proxy listening under {localEndPoint}...");

            TcpClient? client;
            while ((client = await listener.AcceptTcpClientAsync(ct)) != null)
            {
                // Connect to server.
                TcpClient server = new TcpClient();
                server.Connect("uk1.servers.worms2.com", 17171);

                PacketConnection clientConnection = new PacketConnection(client);
                PacketConnection serverConnection = new PacketConnection(server);
                Log(Color.Green, $"{clientConnection.RemoteEndPoint} connected.");

                CancellationTokenSource disconnectCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                _ = Task.WhenAny(
                    Forward(clientConnection, serverConnection, true, disconnectCts.Token),
                    Forward(serverConnection, clientConnection, false, disconnectCts.Token))
                    .ContinueWith((antecedent) => disconnectCts.Cancel());
            }
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void Log(Color color, string message)
            => ColorConsole.WriteLine(color, $"{DateTime.Now:HH:mm:ss} {message}");

        private static async Task Forward(PacketConnection from, PacketConnection to, bool fromClient,
            CancellationToken ct)
        {
            string prefix = fromClient
                ? $"{from.RemoteEndPoint} >> {to.RemoteEndPoint}"
                : $"{to.RemoteEndPoint} << {from.RemoteEndPoint}";
            try
            {
                while (true)
                {
                    Packet packet = await from.Read(ct);
                    Log(fromClient ? Color.Cyan : Color.Magenta, $"{prefix} {packet}");
                    await to.Write(packet, ct);
                }
            }
            catch (Exception ex)
            {
                Log(Color.Red, $"{prefix} closed. {ex.Message}");
            }
        }
    }
}
