using BrainWaves.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BrainWaves.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private bool isDarkThemeOn = false;
        private List<string> availableLanguages;
        private string selectedLanguage;
        private bool firstStartup = true;

        public SettingsViewModel()
        {
            Title = Resources.Strings.Resource.Settings;

            AvailableLanguages = new List<string>
            {
                Resources.Strings.Resource.LanguageENG,
                Resources.Strings.Resource.LanguagePL
            };

            selectedLanguage = Preferences.Get(Constants.PrefsCurrentLanguage, 
                new CultureInfo(Constants.EnglishLanguageCode, false).Name);

            OSAppTheme currentTheme = Application.Current.RequestedTheme;
            if (currentTheme == OSAppTheme.Light)
            {
                IsDarkThemeOn = false;
                Preferences.Set(Constants.PrefsTheme, Constants.PrefsLightTheme);
            }
            else
            {
                IsDarkThemeOn = true;
                Preferences.Set(Constants.PrefsTheme, Constants.PrefsDarkTheme);
            }
        }

        public bool IsDarkThemeOn
        {
            get => isDarkThemeOn;
            set
            {
                SetProperty(ref isDarkThemeOn, value);
                HandleThemeChange(value);
            }
        }

        public List<string> AvailableLanguages
        {
            get => availableLanguages;
            set => SetProperty(ref availableLanguages, value);
        }

        public string SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                SetProperty(ref selectedLanguage, value);
                HandleLanguageChange(value);
            }
        }

        private void HandleThemeChange(bool value)
        {
            if (value)
            {
                Application.Current.UserAppTheme = OSAppTheme.Dark;
                Preferences.Set(Constants.PrefsTheme, Constants.PrefsDarkTheme);
            }
            else
            {
                Application.Current.UserAppTheme = OSAppTheme.Light;
                Preferences.Set(Constants.PrefsTheme, Constants.PrefsLightTheme);
            }
        }

        private void HandleLanguageChange(string value)
        {
            CultureInfo cultureInfo;
            if (value == Resources.Strings.Resource.LanguageENG)
            {
                cultureInfo = new CultureInfo(Constants.EnglishLanguageCode, false);
            }
            else
            {
                cultureInfo = new CultureInfo(Constants.PolishLanguageCode, false);
            }
            Preferences.Set(Constants.PrefsCurrentLanguage, cultureInfo.Name);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Resources.Strings.Resource.Culture = cultureInfo;

            if (firstStartup)
            {
                firstStartup = false;
            }
            else
            {
                Application.Current.MainPage = new AppShell();
            }
        }
    }
}
