namespace DogeChat.ViewModels
{
    /// <summary>
    /// View-model for network related settings.
    /// </summary>
    public sealed class NetworkSettings : ViewModelBase
    {
        private string _address = "localhost";
        private int _port = 18237;

        /// <summary>
        /// Gets or sets the address of the server.
        /// </summary>
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                RaisePropertyChanged(nameof(Address));
            }
        }

        /// <summary>
        /// Gets or sets the port of the server.
        /// </summary>
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                RaisePropertyChanged(nameof(Port));
            }
        }
    }
}