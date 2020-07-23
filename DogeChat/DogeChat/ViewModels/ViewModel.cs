using DogeChat.Models;
using DogeChat.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace DogeChat.ViewModels
{
    /// <summary>
    /// View-model for the application.
    /// </summary>
    public sealed class ViewModel : ViewModelBase
    {
        private const string SystemProfileImage = "doggo_system.png";

        private static readonly string DefaultUserName = $"Doggo#{DateTime.UtcNow.Millisecond:0000}";

        private NetworkSettings _network = new NetworkSettings();
        private string _name = DefaultUserName;
        private string _currentText = string.Empty;
        private int _isDisposed;
        private readonly Dictionary<string, UserInfo> _users = new Dictionary<string, UserInfo>();
        private readonly INavigation _navigation;

        /// <summary>
        /// Gets the command used for starting a chat server.
        /// </summary>
        public ICommand ListenCommand { get; }

        /// <summary>
        /// Gets the command used for joining to a chat server.
        /// </summary>
        public ICommand ConnectCommand { get; }

        /// <summary>
        /// Gets the command used for sending a message to the chat server.
        /// </summary>
        public ICommand SendMessageCommand { get; }

        /// <summary>
        /// Gets or sets the network settings.
        /// </summary>
        public NetworkSettings Network
        {
            get => _network;
            set
            {
                _network = value;
                RaisePropertyChanged(nameof(Network));
            }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Gets or sets the text currently being typed.
        /// </summary>
        public string CurrentText
        {
            get => _currentText;
            set
            {
                _currentText = value;
                RaisePropertyChanged(nameof(CurrentText));
            }
        }

        /// <summary>
        /// Gets the list of messages received.
        /// </summary>
        public ObservableCollection<MessageViewModel> Messages { get; } = new ObservableCollection<MessageViewModel>();

        /// <summary>
        /// Initializes a new instance of <see cref="ViewModel"/>.
        /// </summary>
        /// <param name="navigation">The instance of <see cref="INavigation"/> used to navigate between pages.</param>
        public ViewModel(INavigation navigation)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            ListenCommand = new Command(JoinAsServer, () => true);
            ConnectCommand = new Command(JoinAsClient, () => true);
            SendMessageCommand = new Command(SendMessage, () => true);
        }

        /// <inheritdoc cref="ViewModelBase.Dispose"/>
        public override void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 1)
            {
                return;
            }

            var chatClient = DependencyService.Get<IChatClient>();
            chatClient.MessageReceived -= OnServerMessageReceived;
            chatClient.Leave();
            chatClient.Dispose();

            DependencyService.Get<IChatServer>().Dispose();

            base.Dispose();
        }

        /// <summary>
        /// Disconnects from the current chat server.
        /// </summary>
        public void LeaveServer()
        {
            var chatClient = DependencyService.Get<IChatClient>();
            chatClient.MessageReceived -= OnServerMessageReceived;
            chatClient.Leave();
        }

        /// <summary>
        /// Shuts down the running chat server.
        /// </summary>
        public void ShutdownServer()
        {
            DependencyService.Get<IChatServer>().ShutdownAsync();
        }

        private void JoinAsServer()
        {
            var address = _network.Address;
            var port = _network.Port;
            var name = Name;
            var chatServer = DependencyService.Get<IChatServer>();

            chatServer.Listen(address, port);
            JoinServer(address, port, name);
        }

        private void JoinAsClient() =>
            JoinServer(_network.Address, _network.Port, Name);

        // TODO Make this method async and await the tasks.
        private void JoinServer(string address, int port, string name)
        {
            var client = DependencyService.Get<IChatClient>();
            client.MessageReceived += OnServerMessageReceived;
            client.JoinAsync(address, port, name);

            _navigation.PushAsync(CreatePage<ChatPage>());
        }

        private void OnServerMessageReceived(object sender, MessageReceivedEventArgs<ServerMessage> e)
        {
            switch (e.Message.MessageTypeCase)
            {
                case ServerMessage.MessageTypeOneofCase.UserJoined:
                    UpdateUserInfo(e.Message.UserJoined.Id, ui => ui.Name = e.Message.UserJoined.Name);
                    AddMessage(SystemProfileImage, $"New doggo {e.Message.UserJoined.Name} joined the talkz. Much welcomed!");
                    break;

                case ServerMessage.MessageTypeOneofCase.MessageReceived:
                    var userInfo = UpdateUserInfo(e.Message.MessageReceived.Id, ui => ui.Name = e.Message.MessageReceived.Name);
                    AddMessage(userInfo.Image, $"{e.Message.MessageReceived.Name} borks: {e.Message.MessageReceived.Message}");
                    break;

                default:
                    Trace.WriteLine($"Ignored an unknown server message of type '{e.Message.MessageTypeCase}'.");
                    break;
            }
        }

        private void SendMessage()
        {
            DependencyService.Get<IChatClient>().SendMessageAsync(new ClientMessage
            {
                SendMessage = new SendMessage
                {
                    Message = CurrentText
                }
            }).ContinueWith(task =>
            {
                if (task.IsCompleted || task.IsCanceled) return;

                AddMessage(SystemProfileImage, $"Failed to bork your message: {task.Exception?.Message}");
            });

            CurrentText = string.Empty;
        }

        private void AddMessage(string image, string text)
        {
            Messages.Add(new MessageViewModel(image, text));

            while (Messages.Count > 100)
            {
                Messages.RemoveAt(0);
            }
        }

        private TPage CreatePage<TPage>() where TPage : ContentPage, new() => new TPage
        {
            BindingContext = this
        };

        private UserInfo UpdateUserInfo(string userId, Action<UserInfo> onUpdate)
        {
            if (_users.TryGetValue(userId, out var userInfo))
            {
                onUpdate(userInfo);
                return userInfo;
            }

            userInfo = new UserInfo
            {
                Image = "doggo_0.png" // TODO Randomly generated profile image.
            };
            onUpdate(userInfo);
            _users[userId] = userInfo;

            return userInfo;
        }
    }
}
