using BrainWaves.Helpers;
using BrainWaves.Views;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BrainWaves.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Variables
        private bool isDarkThemeOn = false;
        private List<string> availableLanguages;
        private string selectedLanguage;
        private bool firstStartup = true;
        private bool isAutomaticServiceChoosingActive;
        private string serviceUUID;
        private string sendCharacteristicUUID;
        private string receiveCharacteristicUUID;
        private int timeToReadMindInMinutes;
        private int samplingFrequency;
        private Color entryTimeToReadMindColor;
        private Color entrySamplingFreqColor;
        private bool shouldCalculateFftOnLoad;
        private int cutoffFreqOfLowPassFilter;
        #endregion

        #region Constructors
        public SettingsViewModel()
        {
            Title = Resources.Strings.Resource.Settings;

            AvailableLanguages = new List<string>
            {
                Resources.Strings.Resource.LanguageENG,
                Resources.Strings.Resource.LanguagePL
            };

            LoadPreferences();

            LoadTheme();

            GoBackCommand = new Command(async () => await GoBack());
        }
        #endregion

        #region INotify Getters and Setters
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

        public bool IsAutomaticServiceChoosingActive
        {
            get => isAutomaticServiceChoosingActive;
            set
            {
                SetProperty(ref isAutomaticServiceChoosingActive, value);
                Preferences.Set(Constants.PrefsAutomaticServiceChossing, value);
            }
        }

        public string ServiceUUID
        {
            get => serviceUUID;
            set
            {
                SetProperty(ref serviceUUID, value);
                Preferences.Set(Constants.PrefsSavedServiceUUID, value);
            }
        }

        public string SendCharacteristicUUID
        {
            get => sendCharacteristicUUID;
            set
            {
                SetProperty(ref sendCharacteristicUUID, value);
                Preferences.Set(Constants.PrefsSavedSendCharacteristicUUID, value);
            }
        }

        public string ReceiveCharacteristicUUID
        {
            get => receiveCharacteristicUUID;
            set
            {
                SetProperty(ref receiveCharacteristicUUID, value);
                Preferences.Set(Constants.PrefsSavedReceiveCharacteristicUUID, value);
            }
        }

        public int TimeToReadMindInMinutes
        {
            get => timeToReadMindInMinutes;
            set
            {
                int tempVal;
                if(value > Constants.MaxTimeToReadInMinutes)
                {
                    tempVal = Constants.MaxTimeToReadInMinutes;
                    EntryTimeToReadMindColor = Color.Red;
                }
                else if(value < Constants.MinTimeToReadInMinutes)
                {
                    tempVal = Constants.MinTimeToReadInMinutes;
                    EntryTimeToReadMindColor = Color.Red;
                }
                else
                {
                    tempVal = value;
                    EntryTimeToReadMindColor = Color.Transparent;
                }
                SetProperty(ref timeToReadMindInMinutes, tempVal);
                Preferences.Set(Constants.PrefsSavedTimeToReadMindInMinutes, tempVal);
            }
        }

        public int SamplingFrequency
        {
            get => samplingFrequency;
            set
            {
                int tempVal;
                if (value > Constants.MaxSamplingFrequency)
                {
                    tempVal = Constants.MaxSamplingFrequency;
                    EntrySamplingFreqColor = Color.Red;
                }
                else if (value < Constants.MinSamplingFrequency)
                {
                    tempVal = Constants.MinSamplingFrequency;
                    EntrySamplingFreqColor = Color.Red;
                }
                else
                {
                    tempVal = value;
                    EntrySamplingFreqColor = Color.Transparent;
                }
                SetProperty(ref samplingFrequency, tempVal);
                Preferences.Set(Constants.PrefsSavedSamplingFrequency, tempVal);
            }
        }

        public Color EntryTimeToReadMindColor
        {
            get => entryTimeToReadMindColor;
            set => SetProperty(ref entryTimeToReadMindColor, value);
        }

        public Color EntrySamplingFreqColor
        {
            get => entrySamplingFreqColor;
            set => SetProperty(ref entrySamplingFreqColor, value);
        }

        public bool ShouldCalculateFftOnLoad
        {
            get => shouldCalculateFftOnLoad;
            set
            {
                SetProperty(ref shouldCalculateFftOnLoad, value);
                Preferences.Set(Constants.PrefsShouldCalculateFFT, value);
            }
        }

        public int CutoffFreqOfLowPassFilter
        {
            get => cutoffFreqOfLowPassFilter;
            set
            {
                int tempVal;
                if (value < 0)
                {
                    tempVal = Constants.DefaultLowPassFilterMaxFreq;
                }
                else
                {
                    tempVal = value;
                }
                SetProperty(ref cutoffFreqOfLowPassFilter, tempVal);
                Preferences.Set(Constants.PrefsCutoffFreqOfLowPassFilter, tempVal);
            }
        }
        
        #endregion

        #region Functions
        public void HandleThemeChange(bool value)
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

        private void LoadTheme()
        {
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

        public void HandleLanguageChange(string value)
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
                Application.Current.MainPage = new NavigationPage(new ScanPage());
            }
        }

        private void LoadPreferences()
        {
            selectedLanguage = Preferences.Get(Constants.PrefsCurrentLanguage,
                new CultureInfo(Constants.EnglishLanguageCode, false).Name);

            isAutomaticServiceChoosingActive = Preferences.Get(Constants.PrefsAutomaticServiceChossing, true);
            serviceUUID = Preferences.Get(Constants.PrefsSavedServiceUUID, Constants.UartGattServiceId.ToString());
            sendCharacteristicUUID = Preferences.Get(Constants.PrefsSavedSendCharacteristicUUID, Constants.GattCharacteristicReceiveId.ToString());
            receiveCharacteristicUUID = Preferences.Get(Constants.PrefsSavedReceiveCharacteristicUUID, Constants.GattCharacteristicSendId.ToString());

            timeToReadMindInMinutes = Preferences.Get(Constants.PrefsSavedTimeToReadMindInMinutes, Constants.MinTimeToReadInMinutes);
            samplingFrequency = Preferences.Get(Constants.PrefsSavedSamplingFrequency, Constants.MinSamplingFrequency);
            shouldCalculateFftOnLoad = Preferences.Get(Constants.PrefsShouldCalculateFFT, false);
            cutoffFreqOfLowPassFilter = Preferences.Get(Constants.PrefsCutoffFreqOfLowPassFilter, Constants.DefaultLowPassFilterMaxFreq);
        }
        #endregion
    }
}
