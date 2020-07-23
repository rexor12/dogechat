using Android.Util;
using Grpc.Core;
using DogeChat.Droid.Network.Client;
using DogeChat.Network;
using DogeChat.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ChatClient))]
namespace DogeChat.Droid.Network.Client
{
    /// <summary>
    /// An <see cref="IChatClient"/> capable of communicating with a remote chat server.
    /// </summary>
    public sealed class ChatClient : IChatClient
    {
        private IConnection<ClientMessage, ServerMessage> _connection;
        private int _isDisposed;

        /// <inheritdoc cref="IChatClient.MessageReceived"/>
        public event EventHandler<MessageReceivedEventArgs<ServerMessage>> MessageReceived;

        /// <inheritdoc cref="IChatClient.JoinAsync"/>
        public Task JoinAsync(string address, int port, string name)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(nameof(address), address);
            ExceptionHelper.ThrowIfOutOfRange(nameof(port), port, 0, 65535);
            ExceptionHelper.ThrowIfNullOrWhiteSpace(nameof(name), name);

            if (Interlocked.CompareExchange(ref _isDisposed, 1, 1) == 1)
            {
                throw new ObjectDisposedException(nameof(ChatClient));
            }

            if (_connection != null)
            {
                throw new InvalidOperationException("The client is already connected to a server.");
            }

            Log.Info(nameof(ChatClient), $"Creating new connection to '{address}:{port}'...");
            // TODO Research why this is throwing an exception complaining about "http" and "https" protocols being supported only.
            //var channel = GrpcChannel.ForAddress($"{host}:{port}");
            var channel = new Channel($"{address}:{port}", ChannelCredentials.Insecure);
            _connection = new Connection(channel);
            _connection.MessageReceived += MessageReceived;
            Log.Info(nameof(ChatClient), $"Successfully created new connection to '{address}:{port}'.");
            return _connection.SendMessageAsync(new ClientMessage
            {
                Join = new Join
                {
                    Name = name
                }
            });
        }

        /// <inheritdoc cref="IChatClient.Leave"/>
        public void Leave()
        {
            if (Interlocked.CompareExchange(ref _isDisposed, 1, 1) == 1)
            {
                throw new ObjectDisposedException(nameof(ChatClient));
            }

            Log.Info(nameof(ChatClient), "Disconnecting from the server...");
            var connection = Interlocked.Exchange(ref _connection, null);
            connection?.Dispose();
            Log.Info(nameof(ChatClient), "Successfully disconnected from the server.");
        }

        /// <inheritdoc cref="IChatClient.SendMessageAsync"/>
        public Task SendMessageAsync(ClientMessage message)
        {
            if (Interlocked.CompareExchange(ref _isDisposed, 1, 1) == 1)
            {
                throw new ObjectDisposedException(nameof(ChatClient));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return _connection.SendMessageAsync(message);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 1)
            {
                return;
            }

            _connection?.Dispose();
            MessageReceived = null;
        }
    }
}