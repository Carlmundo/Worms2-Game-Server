using System;
using System.Net;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents the main class of the application containing the program entry point.
    /// </summary>
    internal class Program
    {
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static void Main(string[] args)
        {
            string? argEndPoint = args.Length > 0 ? args[0] : null;

            Server server = new Server();
            server.Run(ParseEndPoint(argEndPoint, new IPEndPoint(IPAddress.Any, 17000)));

            //Proxy.Run(ParseEndPoint(argEndPoint, new IPEndPoint(IPAddress.Any, 17001)));
        }

        private static IPEndPoint ParseEndPoint(string? s, IPEndPoint fallback)
        {
            if (UInt16.TryParse(s, out ushort port))
                return new IPEndPoint(fallback.Address, port);
            else if (IPEndPoint.TryParse(s, out IPEndPoint endPoint))
                return endPoint;
            else
                return fallback;
        }
    }
}
