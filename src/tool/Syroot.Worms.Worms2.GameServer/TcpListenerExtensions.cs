using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents extension methods for <see cref="TcpListener"/> instances.
    /// </summary>
    internal static class TcpListenerExtensions
    {
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Accepts a pending connection request as an asynchronous operation.
        /// </summary>
        /// <param name="tcpListener">The <see cref="TcpListener"/> instance.</param>
        /// <returns>The task object representing the asynchronous operation. The <see cref="Task{TcpClient}.Result"/>
        /// property on the task object returns a <see cref="TcpClient"/> used to send and receive data.</returns>
        internal static async Task<TcpClient> AcceptTcpClientAsync(this TcpListener tcpListener,
            CancellationToken cancellationToken = default)
        {
            using (cancellationToken.Register(() => tcpListener.Stop()))
            {
                try
                {
                    return await tcpListener.AcceptTcpClientAsync();
                }
                catch (InvalidOperationException)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    throw;
                }
            }
        }
    }
}
