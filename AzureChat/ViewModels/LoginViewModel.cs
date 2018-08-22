using System.Windows.Input;
using AzureChat.Managers;
using AzureChat.Views;
using Xamarin.Forms;

namespace AzureChat.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {
            this.LogInCommand = new Command(this.LogIn);
        }



        #region Properties

        /// <summary>
        /// Uživatelské jméno
        /// </summary>
        private string username;
        public string Username
        {
            get => this.username;
            set
            {
                this.username = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Probíhá načítání
        /// </summary>
        private bool isLoading;
        public bool IsLoading
        {
            get { return this.isLoading; }
            set
            {
                this.isLoading = value;
                this.OnPropertyChanged();
            }
        }

        #endregion



        #region Commands

        /// <summary>
        /// Command pro přihlášení
        /// </summary>
        public ICommand LogInCommand { get; private set; }
        private async void LogIn()
        {
            this.IsLoading = true;
            if (string.IsNullOrEmpty(this.Username))
            {
                await App.Current.MainPage.DisplayAlert(null, "Please enter your username.", "OK");
                this.IsLoading = false;
                return;
            }

            bool result = await UserManager.Instance.Login(this.Username);

            // při úspěšném přihlášení se MainPage nastaví na PeopleListPage, aby nebyla povolená navigace zpět
            if (result)
            {
                App.Current.MainPage = new NavigationPage(new PeopleListPage());
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "User could not be logged in.", "OK");
            }

            this.IsLoading = false;
        }

        #endregion
    }
}
