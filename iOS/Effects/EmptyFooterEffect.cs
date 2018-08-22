using System;
using AzureChat.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("chatdevdays17")]
[assembly: ExportEffect(typeof(EmptyFooterEffect), "EmptyFooterEffect")]
namespace AzureChat.iOS.Effects
{
    /// <summary>
    /// Ošetřuje, aby se v ListView nezobrazovaly prázdné řádky
    /// </summary>
    public class EmptyFooterEffect : PlatformEffect
    {
        /// <summary>
        /// Volá se, když je komponenta vytvořena
        /// </summary>
        protected override void OnAttached()
        {
            if (this.Control is UITableView)
            {
                (this.Control as UITableView).TableFooterView = new UIView();
            }
            else
            {
                throw new ArgumentException($"Effect EmptyFooterEffect don't support \"{this.Control}\" type");
            }
        }

        /// <summary>
        /// Volá se, když už komponenta není zapotřebí
        /// </summary>
        protected override void OnDetached()
        {
        }
    }
}