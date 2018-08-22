using System;
using AzureChat.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using IconLabel = AzureChat.Controls.IconLabel;

[assembly: ExportRenderer(typeof(IconLabel), typeof(IconLabelRenderer))]
namespace AzureChat.iOS.Renderers
{
    /// <summary>
    /// Renderer pro label s custom fontem, nalezne soubor s názvem odpovídajícím FontFamily
    /// </summary>
    public class IconLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (Element != null && Control != null)
            {
                var font = LoadFontFromName(Element.FontFamily, (float) Element.FontSize);
                this.Control.Font = font;
            }
        }

        /// <summary>
        /// Načte font ze zadaných parametrů
        /// </summary>
        /// <param name="fontFamily">Název souboru s fontem</param>
        /// <param name="fontSize">Velikost textu</param>
        /// <returns></returns>
        private static UIFont LoadFontFromName(string fontFamily, float fontSize)
        {
            string finalFontName = null;

            if (fontFamily == "materialFont")
            {
                finalFontName = "Material Icons";
            }
            else if (fontFamily == "iconFont")
            {
                finalFontName = "IOS8-Icons-Regular";
            }

            if (!string.IsNullOrEmpty(finalFontName))
            {
                var font = UIFont.FromName(finalFontName, fontSize);
                return font;
            }
            else
            {
                throw new ArgumentException(
                    $"Font \"{fontFamily}\" not found. You have to include font to project and set correct build action.");
            }
        }
    }
}