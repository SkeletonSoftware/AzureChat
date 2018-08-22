using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AzureChat.Managers;
using AzureChat.Models;
using AzureChat.ViewModels.ItemViewModels;
using Xamarin.Forms;

namespace AzureChat.ViewModels
{
    class ChatViewModel : BaseViewModel
    {
        #region Events

        /// <summary>
        /// Událost oznamující načtení starších zpráv v chatu a jejich počet
        /// </summary>
        public event EventHandler<int> OlderChatItemsLoaded;
        private void OnOlderChatItemsLoaded(int position)
        {
            this.OlderChatItemsLoaded?.Invoke(this, position);
        }

        /// <summary>
        /// Událost volaná při prvním načtení zpráv nebo odeslání nové zprávy
        /// </summary>
        public event EventHandler DidCompleteInitialLoad;
        private void OnDidCompleteInitialLoad()
        {
            this.DidCompleteInitialLoad?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Událost volaná při stažení nově přijatých zpráv
        /// </summary>
        public event EventHandler NewMessagesLoaded;
        private void OnNewMessagesLoaded()
        {
            this.NewMessagesLoaded?.Invoke(this, EventArgs.Empty);
        }

        #endregion



        #region Fields

        private const int LoopMiliseconds = 1000 * 7; //3 s - doba aktualizační smyčky
        private const int BatchSize = 20; // velikost dávky při stažení zpráv

        private Person recipient; // příjemce v chatu
        private CancellationTokenSource tokenSource; // token pro aktualizační smyčku
        private bool initialLoadDone = false; // bylo provedeno prvotní načtení dat
        private bool endWasReached = false; // byly už načteny všechny staré zprávy

        #endregion



        public ChatViewModel(Person recipient)
        {
            this.recipient = recipient;
            this.SendMessageCommand = new Command(this.SendMessage);

            this.Items = new ObservableCollection<MessageViewModel>();
        }



        #region Properties

        /// <summary>
        /// Kolekce stažených zpráv
        /// </summary>
        private ObservableCollection<MessageViewModel> items;
        public ObservableCollection<MessageViewModel> Items
        {
            get { return this.items; }
            set
            {
                this.items = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Obsah nové zprávy
        /// </summary>
        private string newMessage;
        public string NewMessage
        {
            get { return newMessage; }
            set
            {
                newMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Probíhá načítání
        /// </summary>
        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                OnPropertyChanged();

            }
        }

        /// <summary>
        /// Aktuálně vybraná zpráva
        /// </summary>
        public MessageViewModel SelectedItem
        {
            // zabráníme GUI, aby nechávalo vybrané položky
            get { return null; }
            set
            {
                if (value != null)
                {
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion



        #region Commands

        /// <summary>
        /// Command pro odeslání zprávy
        /// </summary>
        public ICommand SendMessageCommand { get; private set; }
        private async void SendMessage()
        {
            if (string.IsNullOrEmpty(this.NewMessage?.Trim()))
            {
                await App.Current.MainPage.DisplayAlert(null, "Message cannot be empty.", "OK");
                return;
            }

            this.IsLoading = true;

            var manager = MessageManager.DefaultManager;
            var result = await manager.SaveMessageAsync(this.NewMessage, this.recipient.Username);

            if (result == false)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Could not send message.", "OK");
            }
            else
            {
                await CallLoadMessagesToBottom();
                this.NewMessage = "";
                this.OnDidCompleteInitialLoad();
            }

            this.IsLoading = false;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Zpracování události volané při zobrazování další zprávy v chatu
        /// </summary>
        /// <param name="item"></param>
        public void ItemAppearing(MessageViewModel item)
        {
            var index = this.Items.IndexOf(item);

            if (index == 0 && initialLoadDone && !IsLoading && !endWasReached)
            {
                this.LoadOlder();
            }

            if (index == this.Items.Count() - 1)
            {
                initialLoadDone = true;
            }
        }

        /// <summary>
        /// Prvotní načtení dat
        /// </summary>
        /// <returns></returns>
        public async Task LoadData()
        {
            IsLoading = true;
            var manager = MessageManager.DefaultManager;

            List<Message> output = await manager.GetMessagesAsync(this.recipient.Username, null, true, BatchSize);

            if (output != null)
            {
                this.Items = LoadItems(output);
                OnDidCompleteInitialLoad();

                if (output.Count < BatchSize)
                {
                    endWasReached = true;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Could not load messages.", "OK");
            }

            IsLoading = false;
        }

        /// <summary>
        /// Načtení starších zpráv
        /// </summary>
        /// <returns></returns>
        private async Task LoadOlder()
        {
            this.IsLoading = true;
            var manager = MessageManager.DefaultManager;
            List<Message> output = null;

            // aby načítací kolečko neproblikávalo moc rychle, je potřeba počkat minimálně sekundu
            var loadingTask = manager.GetMessagesAsync(this.recipient.Username, this.Items.First().Model, false, BatchSize);
            var delayTask = Task.Delay(1000);
            await Task.WhenAll(loadingTask, delayTask);

            output = loadingTask.Result;

            if (output != null && output.Count >= 0)
            {
                var list = LoadItems(output);
                var index = 0;

                foreach (var item in list) // postupně se vloží starší zprávy na začátek chatu
                {
                    this.Items.Insert(index++, item);
                }

                OnOlderChatItemsLoaded(output.Count);

                if (output.Count < BatchSize) // dostali jsme od serveru méně než jsme zadali, a proto jsme na konci
                {
                    endWasReached = true;
                }
            }
            else if (output == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Could not load messages.", "OK");
            }

            this.IsLoading = false;
        }

        /// <summary>
        /// Metoda pro převedení surových dat na MessageViewModely
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private ObservableCollection<MessageViewModel> LoadItems(List<Message> result)
        {
            var output = new ObservableCollection<MessageViewModel>();
            foreach (var item in result)
            {
                output.Add(new MessageViewModel(item));
            }
            return output;
        }

        /// <summary>
        /// Načte nově přijaté zprávy
        /// </summary>
        /// <returns></returns>
        private async Task<int> LoadMessagesToBottom()
        {
            var manager = MessageManager.DefaultManager;
            List<Message> newItems = null;
            Message lastMessage;

            if (this.Items.Count > 0)
            {
                lastMessage = this.Items.Last().Model;
            }
            else
            {
                lastMessage = null;
            }

            newItems = await manager.GetMessagesAsync(this.recipient.Username, lastMessage, true, BatchSize);

            if (newItems?.Count > 0)
            {
                var loaded = this.LoadItems(newItems);

                foreach (var messageViewModel in loaded)
                {
                    this.Items.Add(messageViewModel);
                }
            }
            else if (newItems == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Could not load messages.", "OK");
            }


            return newItems?.Count ?? 0;
        }

        /// <summary>
        /// Ve smyčce po dávkách načte všechny nové přijaté zprávy
        /// </summary>
        /// <param name="loading"></param>
        /// <returns></returns>
        private async Task CallLoadMessagesToBottom(bool loading = true)
        {
            if (!IsLoading)
            {
                IsLoading = loading;
                int count;

                do
                {
                    count = await LoadMessagesToBottom();
                } while (count >= BatchSize);

                IsLoading = false;
            }
        }

        #endregion



        #region Refresh loop

        /// <summary>
        /// Spustí aktualizační smyčku chatu
        /// </summary>
        public void StartRefreshLoop()
        {
            this.tokenSource = new CancellationTokenSource();
            this.RefreshLoop(tokenSource.Token);
        }

        /// <summary>
        /// Zastaví aktualizační smyčku chatu
        /// </summary>
        public void StopRefreshLoop()
        {
            if (this.tokenSource != null)
            {
                this.tokenSource.Cancel();
                this.tokenSource = null;
            }
        }

        /// <summary>
        /// Aktualizační smyčka chatu
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task RefreshLoop(CancellationToken token)
        {
            while (token != null && !token.IsCancellationRequested)
            {
                await Task.Delay(LoopMiliseconds); // čekání

                if (token != null && !token.IsCancellationRequested)
                {
                    var manager = MessageManager.DefaultManager;

                    if (this.Items?.Count > 0)
                    {
                        // zeptáme se serveru, jestli má novější zprávy
                        bool result = await manager.IsNewMessageAsync(this.recipient.Username, this.Items.Last().Model);

                        if (result && token != null && !token.IsCancellationRequested)
                        {
                            await CallLoadMessagesToBottom(false);
                            OnNewMessagesLoaded();
                        }
                    }
                    else
                    {
                        if (token != null && !token.IsCancellationRequested)
                        {
                            await CallLoadMessagesToBottom(false);
                            OnNewMessagesLoaded();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
