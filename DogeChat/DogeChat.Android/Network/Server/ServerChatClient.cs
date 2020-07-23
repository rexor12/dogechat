using Android.Util;
using DogeChat.Network;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogeChat.Droid.Network.Server
{
    /// <summary>
    /// A service that represents a client connected to the server.
    /// </summary>
    public interface IServerChatClient : IDisposable
    {
        /// <summary>
        /// Gets the unique identifier of the client.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Sends the given <paramref name="message"/> to the connected client.
        /// </summary>
        /// <param name="message">The <see cref="ServerMessage"/> to be sent.</param>
        /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
        Task SendMessageAsync(ServerMessage message);

        /// <summary>
        /// Handles the given <paramref name="message"/> received from the connected client.
        /// </summary>
        /// <param name="message">The <see cref="ClientMessage"/> to be handled.</param>
        /// <param name="clientRepository">The repository used to access the other clients.</param>
        /// <returns>An awaitable <see cref="Task"/> that represents the operation.</returns>
        Task HandleMessageAsync(ClientMessage message, IReadOnlyDictionary<Guid, IServerChatClient> clientRepository);
    }

    /// <summary>
    /// An <see cref="IServerChatClient"/> that represents a client connected to the server.
    /// </summary>
    public sealed class ServerChatClient : IServerChatClient
    {
        private string _name;

        private readonly IAsyncStreamWriter<ServerMessage> _responseStream;

        /// <inheritdoc cref="IServerChatClient.Id"/>
        public Guid Id { get; }

        public ServerChatClient(Guid id, IServerStreamWriter<ServerMessage> responseStream)
        {
            Id = id != Guid.Empty ? id : throw new ArgumentException("The identifier cannot be the default GUID.", nameof(id));
            _responseStream = responseStream ?? throw new ArgumentNullException(nameof(responseStream));
        }

        /// <inheritdoc cref="IServerChatClient.SendMessageAsync"/>
        public Task SendMessageAsync(ServerMessage message) =>
            _responseStream.WriteAsync(message);

        /// <inheritdoc cref="IServerChatClient.HandleMessageAsync"/>
        public Task HandleMessageAsync(ClientMessage message, IReadOnlyDictionary<Guid, IServerChatClient> clientRepository)
        {
            switch (message.MessageTypeCase)
            {
                case ClientMessage.MessageTypeOneofCase.Join:
                    _name = message.Join.Name;
                    return BroadcastAsync(clientRepository, new ServerMessage
                    {
                        UserJoined = new UserJoined
                        {
                            Name = _name
                        }
                    });

                case ClientMessage.MessageTypeOneofCase.SendMessage:
                    return BroadcastAsync(clientRepository, new ServerMessage
                    {
                        MessageReceived = new MessageReceived
                        {
                            Name = _name,
                            Message = message.SendMessage.Message
                        }
                    });

                default:
                    Log.Info(nameof(ServerChatClient), $"Ignored unknown client message of type '{message.MessageTypeCase}'.");
                    return Task.CompletedTask;
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
        }

        private static Task BroadcastAsync(IReadOnlyDictionary<Guid, IServerChatClient> clientRepository, ServerMessage message) =>
            Task.WhenAll(clientRepository.Values.Select(client => client.SendMessageAsync(message)));
    }
}