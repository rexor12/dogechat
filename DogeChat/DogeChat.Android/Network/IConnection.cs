using System;
using System.Threading.Tasks;
using DogeChat.Network;

namespace DogeChat.Droid.Network
{
    /// <summary>
    /// Interface for a network connection.
    /// </summary>
    public interface IConnection : IDisposable
    {

    }

    /// <summary>
    /// Interface for a network connection.
    /// </summary>
    /// <typeparam name="TRequestMessage">The type of the outgoing messages.</typeparam>
    /// <typeparam name="TResponseMessage">The type of the incoming messages.</typeparam>
    public interface IConnection<in TRequestMessage, TResponseMessage> : IConnection
    {
        /// <summary>
        /// An event that is fired when a message is received from the remote end-point.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs<TResponseMessage>> MessageReceived;

        /// <summary>
        /// Sends the given <paramref name="message"/> to the server.
        /// </summary>
        /// <param name="message">The <see cref="TRequestMessage"/> to be sent.</param>
        /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is <c>null</c>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the object is disposed already.</exception>
        Task SendMessageAsync(TRequestMessage message);
    }
}