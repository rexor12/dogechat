namespace DogeChat.ViewModels
{
    /// <summary>
    /// View-model for a message.
    /// </summary>
    public sealed class MessageViewModel : ViewModelBase
    {
        private string _image;
        private string _text;

        /// <summary>
        /// Gets or sets the name of the profile image file.
        /// </summary>
        public string Image
        {
            get => _image;
            set
            {
                _image = value;
                RaisePropertyChanged(nameof(Image));
            }
        }

        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MessageViewModel"/>.
        /// </summary>
        /// <param name="image">The name of the profile image file.</param>
        /// <param name="text">The message text.</param>
        public MessageViewModel(string image, string text)
        {
            Image = image;
            Text = text;
        }
    }
}
