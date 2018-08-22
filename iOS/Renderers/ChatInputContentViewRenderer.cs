using AzureChat.iOS.Renderers;
using UIKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(AzureChat.Controls.ChatInputContentView), typeof(ChatInputContentViewRenderer))]
namespace AzureChat.iOS.Renderers
{
    /// <summary>
    /// Renderer pro view, aby na iOS nezůstávalo pod klávesnicí, ale vyjelo nad ní
    /// </summary>
    public class ChatInputContentViewRenderer : Xamarin.Forms.Platform.iOS.ViewRenderer // Depending on your situation, you will need to inherit from a different renderer
    {
        public ChatInputContentViewRenderer()
        {
            UIKeyboard.Notifications.ObserveWillShow((sender, args) => {

                if (Element != null)
                {
                    Element.Margin = new Thickness(0, 0, 0, args.FrameEnd.Height); // push the entry up to keyboard height when keyboard is activated
                }
            });

            UIKeyboard.Notifications.ObserveWillHide((sender, args) => {

                if (Element != null)
                {
                    Element.Margin = new Thickness(0); // set the margins to zero when keyboard is dismissed
                }

            });
        }

    }
}