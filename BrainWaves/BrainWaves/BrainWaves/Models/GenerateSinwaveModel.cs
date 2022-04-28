using BrainWaves.Helpers;
using Xamarin.Essentials;

namespace BrainWaves.Models
{
    public class GenerateSinwaveModel : ModelWithINotify
    {
        private int sinwaveSamplingFreq;
        private int sinwaveLength;
        private int sinwaveAmplitude;
        private int sinwaveFrequency;
        private bool isGenerateSinwaveVisible = false;
        private bool isReadButtonEnabled = false;

        public GenerateSinwaveModel()
        {
            SinwaveSamplingFreq = Preferences.Get(Constants.PrefsSinwaveSamplingFreq, Constants.DefaultSinwaveSamplingFreq);
            SinwaveLength = Preferences.Get(Constants.PrefsSinwaveLength, Constants.DefaultSinwaveLength);
            SinwaveAmplitude = Preferences.Get(Constants.PrefsSinwaveAmplitude, Constants.DefaultSinwaveAmplitude);
            SinwaveFrequency = Preferences.Get(Constants.PrefsSinwaveFreq, Constants.DefaultSinwaveFreq);
        }

        public int SinwaveSamplingFreq
        {
            get => sinwaveSamplingFreq;
            set
            {
                int tempValue;
                if (value < 0)
                {
                    tempValue = 0;
                }
                else
                {
                    tempValue = value;
                }
                SetProperty(ref sinwaveSamplingFreq, tempValue);
                Preferences.Set(Constants.PrefsSinwaveSamplingFreq, tempValue);
            }
        }

        public int SinwaveLength
        {
            get => sinwaveLength;
            set
            {
                int tempValue;
                if (value < 0)
                {
                    tempValue = 0;
                }
                else
                {
                    tempValue = value;
                }
                SetProperty(ref sinwaveLength, tempValue);
                Preferences.Set(Constants.PrefsSinwaveLength, tempValue);
            }
        }

        public int SinwaveAmplitude // todo change to float
        {
            get => sinwaveAmplitude;
            set
            {
                int tempValue;
                if (value < 0)
                {
                    tempValue = 0;
                }
                else
                {
                    tempValue = value;
                }
                SetProperty(ref sinwaveAmplitude, tempValue);
                Preferences.Set(Constants.PrefsSinwaveAmplitude, tempValue);
            }
        }

        public int SinwaveFrequency
        {
            get => sinwaveFrequency;
            set
            {
                int tempValue;
                if (value < 0)
                {
                    tempValue = 0;
                }
                else
                {
                    tempValue = value;
                }
                SetProperty(ref sinwaveFrequency, tempValue);
                Preferences.Set(Constants.PrefsSinwaveFreq, tempValue);
            }
        }

        public bool IsGenerateSinwaveVisible
        {
            get => isGenerateSinwaveVisible;
            set
            {
                SetProperty(ref isGenerateSinwaveVisible, value);
                IsReadButtonEnabled = !value;
            }
        }

        public bool IsReadButtonEnabled
        {
            get => isReadButtonEnabled;
            set => SetProperty(ref isReadButtonEnabled, value);
        }
    }
}
