using System;
using System.Linq;
using System.Threading.Tasks;
using AzureChat.Models;
using AzureChat.ViewModels;
using AzureChat.ViewModels.ItemViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AzureChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage
    {
        private ChatViewModel viewModel;

        public ChatPage(Person recipient)
        {
            InitializeComponent();
            this.Title = recipient.Username;
            this.viewModel = new ChatViewModel(recipient);
            this.BindingContext = this.viewModel;
        }

        /// <summary>
        /// Při zobrazení stránky se zaregistrují události, načtou data a spustí se aktualizační smyčka chatu
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            this.viewModel.DidCompleteInitialLoad += ViewModel_InitialChatLoaded;
            this.viewModel.OlderChatItemsLoaded += ViewModel_OlderChatItemsLoaded;
            this.viewModel.NewMessagesLoaded += ViewModel_NewMessagesLoaded;
            messageEntry.Focused += this.MessageEntry_Focused;

            await this.viewModel.LoadData();
            this.viewModel.StartRefreshLoop();
            await this.ScrollToBottom();
        }

        /// <summary>
        /// Při odchodu ze stránky se odregistrují události a zastaví aktualizační smyčka chatu
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            this.viewModel.DidCompleteInitialLoad -= ViewModel_InitialChatLoaded;
            this.viewModel.OlderChatItemsLoaded -= ViewModel_OlderChatItemsLoaded;
            this.viewModel.NewMessagesLoaded -= ViewModel_NewMessagesLoaded;
            this.messageEntry.Focused -= this.MessageEntry_Focused;

            this.viewModel.StopRefreshLoop();
        }

        /// <summary>
        /// Nascrolluje chat až dolů na nejnovější zprávu
        /// </summary>
        /// <returns></returns>
        private async Task ScrollToBottom()
        {
            // ----- workaround - pokud se nascrolluje dolů, než se vše dovykreslí (např. klávesnice), občas zaskočí nejnovější zpráva pod klávesnici
            //navíc tím vznikají race conditions a aplikace pak může padat vyjímkou IndexOutOfRange 
            await Task.Delay(300)
                .ContinueWith((task) =>
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (this.viewModel.Items != null && this.viewModel.Items.Count > 0)
                        {
                            this.ChatListView.ScrollTo(this.viewModel.Items.Last(), ScrollToPosition.End, false);
                        }
                    }));
        }

        private async void ViewModel_InitialChatLoaded(object sender, EventArgs e)
        {
            this.messageEntry.Focus();
            await this.ScrollToBottom();
        }

        /// <summary>
        /// Byly načteny starší zprávy, zaručí správné nascrollování
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="itemIndex"></param>
        private void ViewModel_OlderChatItemsLoaded(object sender, int itemIndex)
        {
            if (itemIndex < 0)
            {
                itemIndex = 0;
            }
            if (itemIndex > 0 && itemIndex >= this.viewModel.Items.Count)
            {
                itemIndex = this.viewModel.Items.Count - 1;
            }

            this.ChatListView.ScrollTo(this.viewModel.Items.ElementAt(itemIndex), ScrollToPosition.Start, false);
        }

        /// <summary>
        /// Byly načteny nové zprávy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewModel_NewMessagesLoaded(object sender, EventArgs e)
        {
            await this.ScrollToBottom();
        }

        /// <summary>
        /// Pokud se začne psát zpráva, zascrolluje se dolů
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MessageEntry_Focused(object sender, FocusEventArgs e)
        {
            await this.ScrollToBottom();
        }

        /// <summary>
        /// Volá se při zobrazování další položky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChatListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            this.viewModel.ItemAppearing((e.Item as MessageViewModel));
        }
    }
}