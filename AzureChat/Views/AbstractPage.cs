using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AzureChat.Views
{
    /// <summary>
    /// Celá třída slouží jako workaround pro Xamarin bug, kdy při odchodu ze stránky s viditelnou klávesnicí zůstane na předchoí stránce bílá plocha
    /// </summary>
    public abstract class AbstractPage : ContentPage
    {
        private static bool pageWasLeftWithOpenKeyboard;

        protected AbstractPage()
        {
            this.IsKeyboardVisible = false;
        }

        /// <summary>
        /// Metoda je volána, když se stránka objeví na displeji.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.IsActive = true;
        }

        /// <summary>
        /// Metoda je volána když se stránka zmizí z displeje. Je určena pro odhlášení eventů.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.IsActive = false;
            pageWasLeftWithOpenKeyboard = this.IsKeyboardVisible;
        }

        /// <summary>
        /// Volá se když se počítají rozměry stránky
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            this.CheckKeyboard(height, width);

            //Je potřeba aby se velikost nastavovala vždy včetně změny orientace, vysunutí klávesnice atd. 
            this.PageHeight = height;
            this.PageWidth = width;
        }

        /// <summary>
        /// Metoda je volána když se zobrazí/skryje klávesnice
        /// </summary>
        /// <returns></returns>
        public virtual void OnKeyboardShown(double keyboardHeight)
        {
            this.IsKeyboardVisible = true;
        }

        /// <summary>
        /// Metoda je volána, když se zobrazí/skryje klávesnice
        /// </summary>
        /// <returns></returns>
        public virtual async void OnKeyboardHidden(double lastkeyboardHeight)
        {
            this.IsKeyboardVisible = false;
            if (Xamarin.Forms.Device.RuntimePlatform == Device.Android)
            {
                if (this.IsActive && pageWasLeftWithOpenKeyboard)
                {
                    var a = this.Content;
                    this.Content = new ActivityIndicator()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        IsEnabled = true,
                        IsRunning = true,
                        IsVisible = true
                    };
                    await Task.Yield();
                    this.Content = a;

                    pageWasLeftWithOpenKeyboard = false;
                }
            }
        }

        private void CheckKeyboard(double height, double width)
        {
            if (Device.RuntimePlatform == Device.Android) //Na Androidu identifikuje vysunutí a schování klávesnice
            {
                if (this.PageHeight > 0 && this.PageWidth > 0)
                {
                    if (width == this.PageWidth && this.CheckKeyboardHeighTolerance(height))
                    {
                        if (this.PageHeight > height)
                        {
                            this.OnKeyboardShown(this.PageHeight - height);
                        }
                        else
                        {
                            this.OnKeyboardHidden(height - this.PageHeight);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Vyhodnocuje, zda je změna výšky dostatečná na to, aby byla událost považována za vysunutí klávesnice
        /// </summary>
        /// <param name="height"></param>
        /// <returns>True - jedná se o vysunutí klávesnice; False - nejedná se o vysunutí klávesnice</returns>
        private bool CheckKeyboardHeighTolerance(double height)
        {
            const double keyboardToleration = 40; //Pokud je rozdíl ve výšce jen tato hodnota nepovažujeme to za klávesnici
            var keyboardHeight = Math.Abs(height - this.PageHeight);
            return keyboardHeight > keyboardToleration;
        }


        /// <summary>
        /// Určuje, zda je stránka v popředí, či nikoliv
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Šířka stránky
        /// </summary>
        protected double PageWidth { get; private set; }

        /// <summary>
        /// Výška stránky (s vysunutou klávesnicí je zde výška zobrazované oblasti bez klávesnice)
        /// </summary>
        protected double PageHeight { get; private set; }

        /// <summary>
        /// Zda je klávesnice zobrazena
        /// </summary>
        protected bool IsKeyboardVisible { get; private set; }
    }
}
