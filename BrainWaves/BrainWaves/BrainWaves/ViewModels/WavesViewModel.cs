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
        private List<List<BrainWaveSample>> brainWaveSamples;
        #endregion

        #region ICommands
        public ICommand GoToSettingsCommand { private set; get; }
        #endregion

        #region Constructors
        public WavesViewModel()
        {
            Title = Resources.Strings.Resource.WavesTitle;
            brainWaveSamples = new List<List<BrainWaveSample>>();
            for(int i = 0; i < 6; i++)
            {
                List<BrainWaveSample> samples = TestSamplesGenerator.GenerateBrainWaveSamples(1000);
                brainWaveSamples.Add(samples);
            }
            InitCommands();
            InitCharts();
        }
        public WavesViewModel(List<List<BrainWaveSample>> samples)
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

            for (int i = 0; i < brainWaveSamples.Count; i++)
            {
                double average = 0;
                switch (type)
                {
                    case ChartType.ALFA:
                        foreach (var sample in brainWaveSamples[i])
                            average += sample.AlfaWave;
                        break;
                    case ChartType.BETA:
                        foreach (var sample in brainWaveSamples[i])
                            average += sample.BetaWave;
                        break;
                    case ChartType.THETA:
                        foreach (var sample in brainWaveSamples[i])
                            average += sample.ThetaWave;
                        break;
                    case ChartType.DELTA:
                        foreach (var sample in brainWaveSamples[i])
                            average += sample.DeltaWave;
                        break;
                }
                
                average /= brainWaveSamples[i].Count; // todo maybe change average to sth else
                samples.Add(HelperFunctions.GenerateChartEntryForWaves(average, i));
            }

            switch (type)
            {
                case ChartType.ALFA:
                    AlfaWavesChart = HelperFunctions.GenerateBarChart(samples, Orientation.Horizontal);
                    break;
                case ChartType.BETA:
                    BetaWavesChart = HelperFunctions.GenerateBarChart(samples, Orientation.Horizontal);
                    break;
                case ChartType.THETA:
                    ThetaWavesChart = HelperFunctions.GenerateBarChart(samples, Orientation.Horizontal);
                    break;
                case ChartType.DELTA:
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
