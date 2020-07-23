using Grpc.Core;
using DogeChat.Extensions;
using DogeChat.Network;
using DogeChat.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Util;

namespace DogeChat.Droid.Network.Client
{
    /// <summary>
    /// Client-side implementation of <see cref="IConnection{TRequestMessage,TResponseMessage}"/>.
    /// </summary>
    public sealed class Connection : IConnection<ClientMessage, ServerMessage>
    {
        private int _isDisposed;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly Channel _channel;
        private readonly AsyncDuplexStreamingCall<ClientMessage, ServerMessage> _duplexStream;
        private readonly Task _subscription;

        /// <summary>
        /// An event that is fired when a message is received from the remote end-point.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<ServerMessage>> MessageReceived;

        /// <summary>
        /// Initializes a new instance of <see cref="Connection"/>.
        /// </summary>
        /// <param name="channel">The <see cref="Channel"/> to be used for communication.</param>
        public Connection(Channel channel)
        {
            ExceptionHelper.ThrowIfNull(nameof(channel), channel);

            var client = new ChatService.ChatServiceClient(channel);
            _channel = channel;
            _duplexStream = client.Subscribe();
            _subscription = ReadMessagesAsync(_tokenSource.Token);
        }

        /// <inheritdoc cref="IConnection{TRequestMessage,TResponseMessage}.SendMessageAsync"/>
        public Task SendMessageAsync(ClientMessage message)
        {
            ExceptionHelper.ThrowIfNull(nameof(message), message);

            if (_isDisposed == 1)
            {
                throw new ObjectDisposedException(nameof(Connection));
            }

            return _duplexStream.RequestStream.WriteAsync(message);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 1)
            {
                return;
            }

            _tokenSource.Cancel();
            try
            {
                _duplexStream.RequestStream.CompleteAsync().Wait();
                _channel.ShutdownAsync().Wait();
            }
            finally
            {
                _duplexStream.Dispose();
                //_channel.Dispose();
                _tokenSource.Dispose();

                MessageReceived = null;
            }

            _subscription.WaitSafely();
        }

        private async Task ReadMessagesAsync(CancellationToken token)
        {
            // TODO Handle the case this task stops (probably a disconnection).
            await foreach (var message in _duplexStream.ResponseStream.ReadAllAsync(token).ConfigureAwait(false))
            {
                var @event = MessageReceived;
                @event?.Invoke(this, new MessageReceivedEventArgs<ServerMessage>(message));
            }

            Log.Warn(nameof(Connection), "The message consumer has stopped.");
        }
    }
}