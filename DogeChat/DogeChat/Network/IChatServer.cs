using System;
using System.Threading.Tasks;

namespace DogeChat.Network
{
    /// <summary>
    /// Interface for a service capable of serving <see cref="IChatClient"/> instances.
    /// </summary>
    public interface IChatServer : IDisposable
    {
        /// <summary>
        /// Binds the server to the given <paramref name="address"/> and <paramref name="port"/>
        /// and begins listening to incoming requests.
        /// </summary>
        /// <param name="address">The address to bind to.</param>
        /// <param name="port">The port to bind to.</param>
        /// <exception cref="ArgumentException">Thrown when one of the arguments has an invalid value.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one of the argument values falls outside the valid range.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the server is listening already.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the object is disposed already.</exception>
        void Listen(string address, int port);

        /// <summary>
        /// Shuts down the server, stopping to process incoming requests.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the object is disposed already.</exception>
        Task ShutdownAsync();
    }
}
