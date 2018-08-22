using AzureChat.ViewModels;

namespace AzureChat.Views
{
    public partial class PeopleListPage
    {
        private PeopleListViewModel viewModel;

        public PeopleListPage()
        {
            InitializeComponent();
            this.viewModel = new PeopleListViewModel();
            this.BindingContext = this.viewModel;
        }

        /// <summary>
        /// Při zobrazení stránky se provede načtení dat
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await this.viewModel.LoadData();
        }
    }
}