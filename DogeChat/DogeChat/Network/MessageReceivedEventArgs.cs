namespace DogeChat.Network
{
    /// <summary>
    /// Event arguments used when a message is received from the remote end-point.
    /// </summary>
    /// <typeparam name="TMessage">The type of the received message.</typeparam>
    public sealed class MessageReceivedEventArgs<TMessage>
    {
        /// <summary>
        /// Gets the received messages.
        /// </summary>
        public TMessage Message { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="MessageReceivedEventArgs{TMessage}"/>.
        /// </summary>
        /// <param name="message">The <see cref="TMessage"/> received from the remote end-point.</param>
        public MessageReceivedEventArgs(TMessage message)
        {
            Message = message;
        }
    }
}
