using BrainWaves.Helpers;
using BrainWaves.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Globalization;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves
{
    public partial class App : Application
    {

        public App()
        {
            string language = Preferences.Get(Constants.PrefsCurrentLanguage, new CultureInfo(Constants.EnglishLanguageCode, false).Name);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language, false);
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static async void ClosePopup()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
