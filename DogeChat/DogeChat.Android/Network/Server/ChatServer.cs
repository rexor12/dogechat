using Android.Util;
using Grpc.Core;
using DogeChat.Droid.Network.Server;
using DogeChat.Extensions;
using DogeChat.Network;
using DogeChat.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ChatServer))]
namespace DogeChat.Droid.Network.Server
{
    /// <summary>
    /// A service capable of serving remote chat clients.
    /// </summary>
    public sealed class ChatServer : IChatServer
    {
        private Grpc.Core.Server _server;
        private int _isDisposed;

        /// <inheritdoc cref="IChatServer.Listen"/>
        public void Listen(string address, int port)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(nameof(address), address);
            ExceptionHelper.ThrowIfOutOfRange(nameof(port), port, 0, 65535);

            if (_isDisposed == 1)
            {
                throw new ObjectDisposedException(nameof(ChatServer));
            }

            if (_server != null)
            {
                throw new InvalidOperationException("The server is listening already.");
            }

            Log.Info(nameof(ChatServer), $"Starting server on '{address}:{port}'...");
            _server = new Grpc.Core.Server
            {
                Services = { ChatService.BindService(new GrpcChatService()) },
                Ports = { new ServerPort(address, port, ServerCredentials.Insecure) }
            };
            _server.Start();
            Log.Info(nameof(ChatServer), $"Started server on '{address}:{port}'.");
        }

        /// <inheritdoc cref="IChatServer.ShutdownAsync"/>
        public async Task ShutdownAsync()
        {
            if (_isDisposed == 1)
            {
                throw new ObjectDisposedException(nameof(ChatServer));
            }

            Log.Info(nameof(ChatServer), "Shutting down server...");
            var server = Interlocked.Exchange(ref _server, null);
            await server.ShutdownAsync().ConfigureAwait(false);
            Log.Info(nameof(ChatServer), "Shutdown complete.");
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 1)
            {
                return;
            }

            _server?.ShutdownAsync().WaitSafely();
        }
    }
}