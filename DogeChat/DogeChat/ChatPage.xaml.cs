using DogeChat.Network;
using DogeChat.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DogeChat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ChatPage"/>.
        /// </summary>
        public ChatPage()
        {
            InitializeComponent();
        }

        /// <inheritdoc cref="Page.OnDisappearing"/>
        protected override void OnDisappearing()
        {
            if (BindingContext is ViewModel vm)
            {
                vm.LeaveServer();
                vm.ShutdownServer();
            }

            base.OnDisappearing();
        }
    }
}