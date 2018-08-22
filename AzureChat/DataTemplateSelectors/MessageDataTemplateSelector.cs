using AzureChat.ViewModels.ItemViewModels;
using Xamarin.Forms;

namespace AzureChat.DataTemplateSelectors
{
    /// <summary>
    /// TemplateSelector pro vybrání templatu zprávy (odeslaná / přijatá)
    /// </summary>
    class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SentMessageTemplate { get; set; }
        public DataTemplate ReceivedMessageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((MessageViewModel)item).IsMine ? SentMessageTemplate : ReceivedMessageTemplate;
        }
    }
}
