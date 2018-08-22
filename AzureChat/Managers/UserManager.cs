using System.Threading.Tasks;
using AzureChat.Models;

namespace AzureChat.Managers
{
    /// <summary>
    /// Manager uchovávající aktuálního uživatele
    /// </summary>
    public class UserManager
    {
        private static UserManager instance;

        private UserManager()
        {
        }

        public static UserManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserManager();
                }
                return instance;
            }
        }

        public Person CurrentUser { get; private set; }

        public bool IsUserLoggedIn => this.CurrentUser != null;

        public async Task<bool> Login(string username)
        {
            var person = new Person() { Username = username };

            var manager = PersonManager.DefaultManager;
            bool result = await manager.SavePersonAsync(person);

            if (result)
            {
                this.CurrentUser = person;
            }

            return result;
        }
    }
}
