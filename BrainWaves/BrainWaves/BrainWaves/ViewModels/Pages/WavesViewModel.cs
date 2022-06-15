using BrainWaves.Helpers;
using BrainWaves.Models;
using Microcharts;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrainWaves.ViewModels
{
    public class WavesViewModel : BaseViewModel
    {
        
        #region Variables
        private Chart alfaWavesChart;
        private Chart thetaWavesChart;
        private Chart betaWavesChart;
        private Chart deltaWavesChart;
        private List<BrainWaveSample> brainWaveSamples;
        #endregion

        #region ICommands
        public ICommand GoToSettingsCommand { private set; get; }
        #endregion

        #region Constructors
        public WavesViewModel()
        {
            Title = Resources.Strings.Resource.WavesTitle;
            brainWaveSamples = TestSamplesGenerator.GenerateBrainWaveSamples(Constants.DefaultNumOfMeasurementsForWaves);
            InitCommands();
            InitCharts();
        }
        public WavesViewModel(List<BrainWaveSample> samples)
        {
            Title = Resources.Strings.Resource.WavesTitle;
            brainWaveSamples = samples; // count should be 6
            InitCommands();
            InitCharts();
        }
        #endregion

        #region INotify Getters and Setters
        public Chart AlfaWavesChart
        {
            get => alfaWavesChart;
            set => SetProperty(ref alfaWavesChart, value);
        }

        public Chart ThetaWavesChart
        {
            get => thetaWavesChart;
            set => SetProperty(ref thetaWavesChart, value);
        }

        public Chart BetaWavesChart
        {
            get => betaWavesChart;
            set => SetProperty(ref betaWavesChart, value);
        }

        public Chart DeltaWavesChart
        {
            get => deltaWavesChart;
            set => SetProperty(ref deltaWavesChart, value);
        }
        #endregion

        #region Functions
        private void InitCommands()
        {
            GoBackCommand = new Command(async () => await GoBack());
            GoToSettingsCommand = new Command(async () => await GoToSettings());
        }

        private void InitCharts()
        {
            InitChart(ChartType.ALFA);
            InitChart(ChartType.BETA);
            InitChart(ChartType.THETA);
            InitChart(ChartType.DELTA);
        }

        private void InitChart(ChartType type)
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.SettingUpChartsMessage;
            List<ChartEntry> samples = new List<ChartEntry>();

            switch (type)
            {
                case ChartType.ALFA:
                    for (int i = 0; i < brainWaveSamples.Count; i++)
                        samples.Add(HelperFunctions.GenerateChartEntryForWaves(brainWaveSamples[i].AlfaWave, i));

                    AlfaWavesChart = HelperFunctions.GenerateBarChart(samples, Orientation.Horizontal);
                    break;
                case ChartType.BETA:
                    for (int i = 0; i < brainWaveSamples.Count; i++)
                        samples.Add(HelperFunctions.GenerateChartEntryForWaves(brainWaveSamples[i].BetaWave, i));

                    BetaWavesChart = HelperFunctions.GenerateBarChart(samples, Orientation.Horizontal);
                    break;
                case ChartType.THETA:
                    for (int i = 0; i < brainWaveSamples.Count; i++)
                        samples.Add(HelperFunctions.GenerateChartEntryForWaves(brainWaveSamples[i].ThetaWave, i));

                    ThetaWavesChart = HelperFunctions.GenerateBarChart(samples, Orientation.Horizontal);
                    break;
                case ChartType.DELTA:
                    for (int i = 0; i < brainWaveSamples.Count; i++)
                        samples.Add(HelperFunctions.GenerateChartEntryForWaves(brainWaveSamples[i].DeltaWave, i));

                    DeltaWavesChart = HelperFunctions.GenerateBarChart(samples, Orientation.Horizontal);
                    break;
            }

            IsBusy = false;
        }
        #endregion

        private enum ChartType
        {
            ALFA,
            BETA,
            THETA,
            DELTA
        }
    }
}
