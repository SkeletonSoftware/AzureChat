using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AzureChat.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(AzureChat.Controls.IconLabel), typeof(IconLabelRenderer))]
namespace AzureChat.Droid.Renderers
{
    /// <summary>
    /// Renderer pro label s custom fontem, nalezne soubor s názvem odpovídajícím FontFamily
    /// </summary>
    class IconLabelRenderer : LabelRenderer
    {
        public IconLabelRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            var typeface = Typeface.CreateFromAsset(this.Context.Assets, System.IO.Path.Combine("fonts", $"{Element.FontFamily}.ttf"));
            Control.Typeface = typeface;
        }
    }
}