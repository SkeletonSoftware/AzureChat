using AzureChat.ViewModels;

namespace AzureChat.Views
{
    public partial class LoginPage
    {
        private LoginViewModel viewModel;

        public LoginPage()
        {
            InitializeComponent();
            this.viewModel = new LoginViewModel();
            this.BindingContext = this.viewModel;
        }
    }
}