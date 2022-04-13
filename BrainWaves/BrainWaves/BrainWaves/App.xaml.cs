using BrainWaves.Helpers;
using BrainWaves.Popups;
using BrainWaves.Views;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BrainWaves
{
    public partial class App : Application
    {
        public static List<float> fSamples = new List<float>();
        public App()
        {
            string language = Preferences.Get(Constants.PrefsCurrentLanguage, new CultureInfo(Constants.EnglishLanguageCode, false).Name);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language, false);
            InitializeComponent();

            MainPage = new NavigationPage(new ChartsPage());
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

        public static async Task OpenInfoPopup(string title, string message)
        {
            await PopupNavigation.Instance.PushAsync(new InfoPopup(title, message));
        }

        public static async void ClosePopup()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
