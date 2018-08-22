using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Microsoft.WindowsAzure.MobileServices;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace AzureChat.Droid
{
	[Activity (Label = "AzureChat.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, Theme = "@style/MainTheme")]
	public class MainActivity : FormsAppCompatActivity
    {
		protected override void OnCreate (Bundle bundle)
		{
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate (bundle);

			// Initialize Azure Mobile Apps
			CurrentPlatform.Init();

			// Initialize Xamarin Forms
			Forms.Init (this, bundle);

		    var app = new App();

		    // kod co opravuje klavesnici. Aby spravne fungoval statusbar a neprekryval aplikaci, tak je nutne aplikaci nastavit WindowSoftInputModeAdjust = "Resize"
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
		    {
		        // unfortunately this creates a transparent statusbar, so 're-color' the status bar again
		        var statusBarHeightInfo = typeof(FormsAppCompatActivity).GetField("_statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		        if (statusBarHeightInfo != null)
		        {
		            statusBarHeightInfo.SetValue(this, 0);
		        }

		        Color color = (Color)app.Resources["PrimaryColor"];
		        Window.SetStatusBarColor(color.ToAndroid());
		    }

            // Load the main application
            LoadApplication(app);

		    Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

		}
	}
}

