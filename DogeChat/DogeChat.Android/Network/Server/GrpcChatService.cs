using Android.Util;
using DogeChat.Extensions;
using DogeChat.Models;
using DogeChat.Network;
using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DogeChat.Droid.Network.Server
{
    /// <summary>
    /// A service that handles incoming gRPC calls for the chat server.
    /// </summary>
    public sealed class GrpcChatService : ChatService.ChatServiceBase
    {
        private readonly ConcurrentDictionary<Identifier, IServerChatClient> _clientRepository = new ConcurrentDictionary<Identifier, IServerChatClient>();

        /// <inheritdoc cref="ChatService.ChatServiceBase.Subscribe"/>
        public override async Task Subscribe(IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            var clientId = (Identifier)Guid.NewGuid();
            Log.Info(nameof(GrpcChatService), $"A new client '{clientId}' has joined the server.");

            using var client = new ServerChatClient(clientId, responseStream);
            _clientRepository[client.Id] = client;
            var consumer = ConsumeMessagesAsync(client, requestStream, context.CancellationToken);

            await consumer.AwaitSafely().ConfigureAwait(false);

            _clientRepository.TryRemove(clientId, out _);

            Log.Info(nameof(GrpcChatService), $"The client '{clientId}' has left the server.");
        }

        private async Task ConsumeMessagesAsync(IServerChatClient client, IAsyncStreamReader<ClientMessage> requestStream, CancellationToken token)
        {
            await foreach (var message in requestStream.ReadAllAsync(token).ConfigureAwait(false))
            {
                try
                {
                    await client.HandleMessageAsync(message, _clientRepository).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Log.Error(nameof(GrpcChatService), $"Something went wrong while broadcasting a message: {e.InnerException?.Message}");
                    throw;
                }
            }
        }
    }
}