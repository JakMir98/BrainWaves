using BrainWaves.Helpers;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BrainWaves.ViewModels
{
    public class GenerateSinwaveViewModel : BaseViewModel
    {
        #region Variables
        private int sinwaveSamplingFreq;
        private int sinwaveLength;
        private int sinwaveAmplitude;
        private int sinwaveFrequency;
        private bool isGenerateSinwaveVisible = false;
        #endregion

        #region ICommands
        public ICommand GenerateCommand { private set; get; }
        #endregion

        #region Constructors
        public GenerateSinwaveViewModel()
        {
            SinwaveSamplingFreq = Preferences.Get(Constants.PrefsSinwaveSamplingFreq, Constants.DefaultSinwaveSamplingFreq);
            SinwaveLength = Preferences.Get(Constants.PrefsSinwaveLength, Constants.DefaultSinwaveLength);
            SinwaveAmplitude = Preferences.Get(Constants.PrefsSinwaveAmplitude, Constants.DefaultSinwaveAmplitude);
            SinwaveFrequency = Preferences.Get(Constants.PrefsSinwaveFreq, Constants.DefaultSinwaveFreq);
            //GenerateCommand = new Command(Generate);
        }
        #endregion

        #region INotify Getters and Setters
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
                //IsReadButtonEnabled = !value;
            }
        }
        #endregion

        #region Functions 
        private async Task<ObservableCollection<double>> Generate()
        {
            await Task.Run(() =>
            {
                //IsReadButtonEnabled = false;
                //IsGoToChartsEnabled = false;
                IsBusy = true;
                BusyMessage = Resources.Strings.Resource.GenerateSinwave;
                double[] sinWave;

                if (FftSharp.Pad.IsPowerOfTwo(SinwaveLength))
                {
                    sinWave = TestSamplesGenerator.GenerateSinWave(
                        SinwaveSamplingFreq, SinwaveLength, SinwaveAmplitude, SinwaveFrequency);
                }
                else
                {// zero padding so array is power of 2
                    sinWave = FftSharp.Pad.ZeroPad(TestSamplesGenerator.GenerateSinWave(
                        SinwaveSamplingFreq, SinwaveLength, SinwaveAmplitude, SinwaveFrequency));
                }

                IsBusy = false;
                //IsGoToChartsEnabled = true;
                //IsReadButtonEnabled = true;

                return new ObservableCollection<double>(sinWave);
                
            });

            return new ObservableCollection<double>();
        }

        #endregion
    }
}
