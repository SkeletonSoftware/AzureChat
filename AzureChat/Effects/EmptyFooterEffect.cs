using Xamarin.Forms;

namespace AzureChat.Effects
{
    /// <summary>
    /// Ošetřuje, aby se v ListView nezobrazovaly prázdné řádky (iOS)
    /// </summary>
    public class EmptyFooterEffect : RoutingEffect
    {
        public EmptyFooterEffect()
            : base("chatdevdays17.EmptyFooterEffect")
        {
        }
    }
}
