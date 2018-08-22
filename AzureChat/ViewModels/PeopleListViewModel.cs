using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AzureChat.Managers;
using AzureChat.Models;
using AzureChat.Views;
using Xamarin.Forms;

namespace AzureChat.ViewModels
{
    public class PeopleListViewModel : BaseViewModel
    {
        public PeopleListViewModel()
        {
            this.RefreshCommand = new Command(this.Refresh);
        }



        #region Methods

        /// <summary>
        /// Provede načtení dat
        /// </summary>
        /// <returns></returns>
        public async Task LoadData()
        {
            this.IsLoading = true;
            var manager = PersonManager.DefaultManager;
            var result = await manager.GetPeopleAsync();

            if (result == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Could not load people.", "OK");
            }
            else
            {
                this.Items = result;
            }

            await Task.Yield(); // potřeba počkat na dodělání práce v hlavním vlákně (zobrazení položek v ListView), aby se poté správně provedlo nastavení IsLoading; bez toho se točí kolečko do nekonečna

            this.IsLoading = false;
        }

        #endregion



        #region Properties

        /// <summary>
        /// Kolekce lidí
        /// </summary>
        private List<Person> items;
        public List<Person> Items
        {
            get { return this.items; }
            set
            {
                this.items = value;
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

        /// <summary>
        /// Aktuálně vybraná osoba
        /// </summary>
        public Person SelectedItem
        {
            // zabráníme GUI, aby nechávalo vybrané položky
            get { return null; }
            set
            {
                if (value != null)
                {
                    App.Current.MainPage.Navigation.PushAsync(new ChatPage(value));
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion



        #region Commands

        /// <summary>
        /// Command pro pull-to-refresh
        /// </summary>
        public ICommand RefreshCommand { get; private set; }
        private async void Refresh()
        {
            await this.LoadData();
        }

        #endregion
    }
}
