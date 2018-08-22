using AzureChat.Managers;
using AzureChat.Models;

namespace AzureChat.ViewModels.ItemViewModels
{
    /// <summary>
    /// ViewModel Message pro zobrazení zpráv v seznamu
    /// </summary>
    class MessageViewModel : BaseViewModel
    {
        private Message message;

        public MessageViewModel(Message data)
        {
            this.message = data;
        }

        public bool IsMine => string.Equals(this.message.Sender, UserManager.Instance.CurrentUser.Username);

        public string Message => this.message.MessageContent;

        public string DisplayDate => this.message.Date.ToString("d.M.yyyy H:mm");

        public Message Model => this.message;
    }
}
