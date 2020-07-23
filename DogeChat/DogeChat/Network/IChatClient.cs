using System;
using System.Threading.Tasks;

namespace DogeChat.Network
{
    /// <summary>
    /// Interface for a service capable of communicating with a remote chat server.
    /// </summary>
    public interface IChatClient : IDisposable
    {
        /// <summary>
        /// An event that is fired when a message is received from the server.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs<ServerMessage>> MessageReceived;

        /// <summary>
        /// Connects to the server specified by the given <paramref name="address"/> and <paramref name="port"/>
        /// and joins the user with the given <paramref name="name"/> to the chat.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="port">The port of the server.</param>
        /// <param name="name">The name of the joining user.</param>
        /// <exception cref="ArgumentException">Thrown when one of the arguments has an invalid value.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one of the argument values falls outside the valid range.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the client is connected already.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the object is disposed already.</exception>
        Task JoinAsync(string address, int port, string name);

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the client isn't connected yet.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the object is disposed already.</exception>
        void Leave();

        /// <summary>
        /// Sends the given <paramref name="message"/> to the server.
        /// </summary>
        /// <param name="message">The <see cref="ClientMessage"/> to be sent.</param>
        /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when one of the arguments is <c>null</c>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the object is disposed already.</exception>
        Task SendMessageAsync(ClientMessage message);
    }
}
