using BrainWaves.Helpers;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;

namespace BrainWaves.ViewModels
{
    public  class TimeFreqMeasurementsViewModel : BaseViewModel
    {
        #region Variables
        private string selectedSettings;
        private List<string> availableSettings;
        private bool isReadButtonEnabled = true;
        private bool isGoToChartsEnabled = false;
        private int samplingFreq;
        private float timeToMeasureInMins;
        #endregion

        #region ICommands
        public ICommand StartCommand { private set; get; }
        public ICommand GoToChartsCommand { private set; get; }
        #endregion

        #region Constructors
        public TimeFreqMeasurementsViewModel()
        {

        }
        #endregion

        #region INotify Getters and Setters
        public string SelectedSettings
        {
            get => selectedSettings;
            set
            {
                SetProperty(ref selectedSettings, value);
                SelectedSettingsChangedHandle(value);
            }
        }

        public List<string> AvailableSettings
        {
            get => availableSettings;
            set => SetProperty(ref availableSettings, value);
        }

        public bool IsReadButtonEnabled
        {
            get => isReadButtonEnabled;
            set => SetProperty(ref isReadButtonEnabled, value);
        }

        public bool IsGoToChartsEnabled
        {
            get => isGoToChartsEnabled;
            set => SetProperty(ref isGoToChartsEnabled, value);
        }
        #endregion

        #region Functions
        private void SelectedSettingsChangedHandle(string setting)
        {
            if (setting == Resources.Strings.Resource.StartSettingTest)
            {
                samplingFreq = 80;
                timeToMeasureInMins = 0.25f;
            }
            else if (setting == Resources.Strings.Resource.StartSettingUserDefined)
            {
                samplingFreq = Preferences.Get(Constants.PrefsSavedSamplingFrequency, Constants.MinSamplingFrequency);
                timeToMeasureInMins = Preferences.Get(Constants.PrefsSavedTimeToReadMindInMinutes, Constants.MinTimeToReadInMinutes);
            }
            else if (setting == Resources.Strings.Resource.StartSettingMinimal)
            {
                samplingFreq = 80;
                timeToMeasureInMins = 1;
            }
            else if (setting == Resources.Strings.Resource.StartSettingBasic)
            {
                samplingFreq = 160;
                timeToMeasureInMins = 15;
            }
            else if (setting == Resources.Strings.Resource.StartSettingShortHighSampleRate)
            {
                samplingFreq = 500;
                timeToMeasureInMins = 5;
            }
            else if (setting == Resources.Strings.Resource.StartSettingLongLowSampleRate)
            {
                samplingFreq = 80;
                timeToMeasureInMins = 30;
            }
            else if (setting == Resources.Strings.Resource.StartSettingAdvanced)
            {
                samplingFreq = 500;
                timeToMeasureInMins = 15;
            }
            else if (setting == Resources.Strings.Resource.StartSettingFull)
            {
                samplingFreq = 500;
                timeToMeasureInMins = 60;
            }
        }
        #endregion

    }
}
