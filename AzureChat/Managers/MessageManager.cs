using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AzureChat.Models;
using Microsoft.WindowsAzure.MobileServices;

namespace AzureChat.Managers
{
    /// <summary>
    /// Manager pro práci s tabulkou message
    /// </summary>
    public class MessageManager
    {
        static MessageManager defaultInstance = new MessageManager();
        MobileServiceClient client;

        IMobileServiceTable<Message> messageTable;

        private MessageManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            this.messageTable = client.GetTable<Message>();
        }

        public static MessageManager DefaultManager
        {
            get => defaultInstance;
            private set => defaultInstance = value;
        }

        public MobileServiceClient CurrentClient => client;

        /// <summary>
        /// Vrátí seznam úplně všech zpráv
        /// </summary>
        /// <returns></returns>
        public async Task<List<Message>> GetMessagesAsync()
        {
            try
            {
                IEnumerable<Message> items = await messageTable.ToEnumerableAsync();

                return items.ToList();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine($"Invalid sync operation: {msioe.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Sync error: {e.Message}");
            }
            return null;
        }

        /// <summary>
        /// Vrátí seznam zpráv
        /// </summary>
        /// <param name="recipient">username druhé osoby v konverzaci</param>
        /// <param name="lastMessage">poslední zpráva (ať už přijatá, nebo odeslaná)</param>
        /// <param name="newer">pokud true, stáhne novější zprávy, jinak starší než lastMessage</param>
        /// <param name="count">velikost stahované dávky</param>
        /// <returns></returns>
        public async Task<List<Message>> GetMessagesAsync(string recipient, Message lastMessage,
            bool newer, int count = 20)
        {
            try
            {
                IEnumerable<Message> items = null;
                var sender = UserManager.Instance.CurrentUser.Username;

                if (lastMessage == null)
                {
                    items = await messageTable
                        .Where(x => (sender == x.Sender && recipient == x.Recipient) ||
                                    (sender == x.Recipient && recipient == x.Sender))
                        .OrderByDescending(x => x.Date)
                        .Take(count)
                        .ToEnumerableAsync();
                }
                else
                {
                    if (newer)
                    {
                        items = await messageTable
                            .Where(x => ((sender == x.Sender && recipient == x.Recipient) ||
                                         (sender == x.Recipient && recipient == x.Sender)) &&
                                        x.Date > lastMessage.Date)
                            .OrderByDescending(x => x.Date)
                            .Take(count)
                            .ToEnumerableAsync();
                    }
                    else
                    {
                        items = await messageTable
                            .Where(x => ((sender == x.Sender && recipient == x.Recipient) ||
                                         (sender == x.Recipient && recipient == x.Sender)) &&
                                        x.Date < lastMessage.Date)
                            .OrderByDescending(x => x.Date)
                            .Take(count)
                            .ToEnumerableAsync();
                    }
                }

                return items.OrderBy(x => x.Date).ToList();
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine($"Invalid sync operation: {msioe.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Sync error: {e.Message}");
            }
            return null;
        }

        /// <summary>
        /// Zjistí, zda jsou k dispozici nové zprávy
        /// </summary>
        /// <param name="recipient">username druhé osoby v konverzaci</param>
        /// <param name="lastMessage">poslední zpráva (ať už přijatá, nebo odeslaná)</param>
        /// <returns></returns>
        public async Task<bool> IsNewMessageAsync(string recipient, Message lastMessage)
        {
            try
            {
                bool isNew = false;
                var sender = UserManager.Instance.CurrentUser.Username;

                var items = await messageTable
                    .Where(x => (sender == x.Sender && recipient == x.Recipient) ||
                                (sender == x.Recipient && recipient == x.Sender))
                    .Where(x => x.Date > lastMessage.Date)
                    .ToEnumerableAsync();

                if (items != null && items.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine($"Invalid sync operation: {msioe.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Sync error: {e.Message}");
            }
            return false;
        }

        /// <summary>
        /// Uloží zprávu do databáze
        /// </summary>
        /// <param name="message">obsah zprávy</param>
        /// <param name="recipient">příjemce zprávy</param>
        /// <returns></returns>
        public async Task<bool> SaveMessageAsync(string message, string recipient)
        {
            Message item = new Message()
            {
                MessageContent = message,
                Recipient = recipient,
                Sender = UserManager.Instance.CurrentUser.Username
            };

            try
            {
                await messageTable.InsertAsync(item);
                return true;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine($"Invalid sync operation: {msioe.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Sync error: {e.Message}");
            }
            return false;
        }
    }
}
