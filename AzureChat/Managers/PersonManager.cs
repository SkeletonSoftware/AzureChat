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
    /// Manager pro práci s tabulkou person
    /// </summary>
    public class PersonManager
    {
        static PersonManager defaultInstance = new PersonManager();
        MobileServiceClient client;

        IMobileServiceTable<Person> personTable;

        private PersonManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            this.personTable = client.GetTable<Person>();
        }

        public static PersonManager DefaultManager
        {
            get => defaultInstance;
            private set => defaultInstance = value;
        }

        public MobileServiceClient CurrentClient => client;

        /// <summary>
        /// Vrátí osobu z databáze s daným username
        /// </summary>
        /// <param name="username">username požadované osoby</param>
        /// <returns></returns>
        public async Task<Person> GetPersonAsync(string username)
        {
            try
            {
                var items = await personTable.Where(x => x.Username == username).ToEnumerableAsync();
                var result = items.FirstOrDefault();
                return result;
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
        /// Vrátí všechny lidi z databáze
        /// </summary>
        /// <returns></returns>
        public async Task<List<Person>> GetPeopleAsync()
        {
            try
            {
                IEnumerable<Person> items = await personTable
                    .Where(x => x.Username != UserManager.Instance.CurrentUser.Username)
                    .OrderBy(x => x.Username)
                    .ToEnumerableAsync();

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
        /// Uloží osobu do databáze
        /// </summary>
        /// <param name="item">instance osoby</param>
        /// <returns></returns>
        public async Task<bool> SavePersonAsync(Person item)
        {
            var person = await this.GetPersonAsync(item.Username);

            if (person == null)
            {
                try
                {
                    await personTable.InsertAsync(item);
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
            return true;
        }
    }
}
