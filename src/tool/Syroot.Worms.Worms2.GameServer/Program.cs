using System;
using System.Net;
using System.Threading.Tasks;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents the main class of the application containing the program entry point.
    /// </summary>
    internal class Program
    {
        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static async Task Main(string[] args)
        {
            string? argEndPoint = args.Length > 0 ? args[0] : null;

            Server server = new Server();
            await server.Run(ParseEndPoint(argEndPoint, new IPEndPoint(IPAddress.Any, 17000)));

            //await Proxy.Run(ParseEndPoint(argEndPoint, new IPEndPoint(IPAddress.Any, 17001)));
        }

        private static IPEndPoint ParseEndPoint(string? s, IPEndPoint fallback)
        {
            if (s == null)
                return fallback;
            else if (UInt16.TryParse(s, out ushort port))
                return new IPEndPoint(fallback.Address, port);
            else if (IPEndPoint.TryParse(s, out IPEndPoint? endPoint))
                return endPoint;
            else
                return fallback;
        }
    }
}
