using BrainWaves.Helpers;
using BrainWaves.Models;
using BrainWaves.Services;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Linq;
using FftSharp;
using System.Globalization;

namespace BrainWaves.ViewModels
{
    public class ChartsViewModel : BaseViewModel
    {
        #region Constants
        private const int MinSampleNum = 288_000;
        private const int MaxSampleNum = 1_800_000;
        #endregion

        #region Variables
        private Chart frequencyChart;
        private Chart timeChart;
        private List<double> timeSamples = new List<double>();
        private ExcelService excelService;
        private bool isFreqChartVisible = false;
        private bool isTimeChartVisible = true;
        private bool isExportButtonEnabled = true;
        private double minSliderValue;
        private double maxSliderValue;
        private double sliderValue;
        
        private int numberOfShownSamplesFromTheMiddle;
        private FrequencySamplesContainer freqSamples;
        private Orientation chartsOrientation;
        private bool shouldExportTimeSamples = true;
        private bool areFreqSamplesReady = false;
        #endregion

        #region ICommands
        public ICommand ExportToExcelCommand { private set; get; }
        public ICommand DragCompletedCommand { private set; get; }
        public ICommand CalculateFFTCommand { private set; get; }
        #endregion

        #region Constructors
        public ChartsViewModel()
        {
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();
            var shouldCalculateFftOnLoad = Preferences.Get(Constants.PrefsShouldCalculateFFT, false);
            if (shouldCalculateFftOnLoad)
            {
                areFreqSamplesReady = false;
            }
            else
            {
                areFreqSamplesReady = true;
            }

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            DragCompletedCommand = new Command(UpdateCharts);
            CalculateFFTCommand = new Command(async () => await FreqChartLoad());

            /* Create samples to test Export to excell */
            int samplingFreq = 500;
            int length = 1048576; //2^20
            double[] sinWave = HelperFunctions.GenerateSinWave(samplingFreq, length, 1, 50);
            timeSamples = new List<double>(sinWave);
            freqSamples = HelperFunctions.GenerateFreqSamples(sinWave, samplingFreq);

            /* Generate random values
            //timeSamples = new List<double>(HelperFunctions.GenerateRandomValues(MinSampleNum));
            */         

            //after samples
            MinSliderValue = 0;
            MaxSliderValue = timeSamples.Count;
            numberOfShownSamplesFromTheMiddle = Preferences.Get(Constants.PrefsSamplesToShowFromMiddle, Constants.DefaultLoadedSamples);
            CheckChartLabelOrientation();
            SetupTimeDomainChart();
            SetupFreqDomainChart();
        }

        public ChartsViewModel(List<double> _samples)
        {
            timeSamples = _samples;
            /*
            double[] timeSamplesArr = _samples.ToArray();
            var samplingFreq = Preferences.Get(Constants.PrefsSavedSamplingFrequency, Constants.MinSamplingFrequency);
            freqSamples = HelperFunctions.GenerateFreqSamples(timeSamplesArr, samplingFreq);
            */
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();
            var shouldCalculateFftOnLoad = Preferences.Get(Constants.PrefsShouldCalculateFFT, false);
            if (shouldCalculateFftOnLoad)
            {
                areFreqSamplesReady = false;
            }
            else
            {
                areFreqSamplesReady = true;
            }

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            DragCompletedCommand = new Command(UpdateCharts);
            CalculateFFTCommand = new Command(async () => await FreqChartLoad());

            MinSliderValue = 0;
            MaxSliderValue = timeSamples.Count;
            numberOfShownSamplesFromTheMiddle = Preferences.Get(Constants.PrefsSamplesToShowFromMiddle, Constants.DefaultLoadedSamples);
            CheckChartLabelOrientation();
            SetupTimeDomainChart();
            //SetupFreqDomainChart();
        }
        #endregion

        #region INotify Getters and Setters
        public Chart FrequencyChart
        {
            get => frequencyChart;
            set => SetProperty(ref frequencyChart, value);
        }

        public Chart TimeChart
        {
            get => timeChart;
            set => SetProperty(ref timeChart, value);
        }

        public bool IsFreqChartVisible
        {
            get => isFreqChartVisible;
            set => SetProperty(ref isFreqChartVisible, value);
        }

        public bool IsTimeChartVisible
        {
            get => isTimeChartVisible;
            set => SetProperty(ref isTimeChartVisible, value);
        }

        public bool IsExportButtonEnabled
        {
            get => isExportButtonEnabled;
            set => SetProperty(ref isExportButtonEnabled, value);
        }

        public double MinSliderValue
        {
            get => minSliderValue;
            set => SetProperty(ref minSliderValue, value);
        }

        public double MaxSliderValue
        {
            get => maxSliderValue;
            set => SetProperty(ref maxSliderValue, value);
        }

        public double SliderValue
        {
            get => sliderValue;
            set => SetProperty(ref sliderValue, value);
        }
        
        public int NumberOfShownSamplesFromTheMiddle
        {
            get => numberOfShownSamplesFromTheMiddle;
            set
            {
                int tempValue;
                if(value > Constants.MaxLoadedSamples)
                {
                    tempValue = Constants.MaxLoadedSamples;
                }
                else
                {
                    tempValue = value;
                }
                SetProperty(ref numberOfShownSamplesFromTheMiddle, tempValue);
                Preferences.Set(Constants.PrefsSamplesToShowFromMiddle, tempValue);
                CheckChartLabelOrientation();
            }
        }

        public bool ShouldExportTimeSamples
        {
            get => shouldExportTimeSamples;
            set => SetProperty(ref shouldExportTimeSamples, value);
        }

        
        public bool AreFreqSamplesReady
        {
            get => areFreqSamplesReady;
            set => SetProperty(ref areFreqSamplesReady, value);
        }
        #endregion

        #region Functions
        #region Charts Handle
        private void CheckChartLabelOrientation()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                if (numberOfShownSamplesFromTheMiddle > Constants.DefaultLoadedSamples - 15)
                {
                    chartsOrientation = Orientation.Vertical;
                }
                else
                {
                    chartsOrientation = Orientation.Horizontal;
                }
            }
            else
            {
                if (numberOfShownSamplesFromTheMiddle > Constants.DefaultLoadedSamples - 10)
                {
                    chartsOrientation = Orientation.Vertical;
                }
                else
                {
                    chartsOrientation = Orientation.Horizontal;
                }
            }            
        }

        private void SetupTimeDomainChart()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.SettingUpChartsMessage;
            List<ChartEntry> timeRecords = new List<ChartEntry>();
            
            int countToLoad = 2 * numberOfShownSamplesFromTheMiddle > timeSamples.Count ? timeSamples.Count : 2 * numberOfShownSamplesFromTheMiddle;
            for (int i = 0; i < countToLoad; i++)
            {
                timeRecords.Add(new ChartEntry((float)Math.Round(timeSamples[i], Constants.NumOfDecimalPlaces))
                {
                    Label = $"{i}.",
                    ValueLabel = $"{Math.Round(timeSamples[i], Constants.NumOfDecimalPlaces)}V",
                    Color = SkiaSharp.SKColor.Parse(Constants.TimeChartColor),
                    TextColor = SKColors.Gray,
                    ValueLabelColor = SKColors.Gray,
                });
            }

            TimeChart = new LineChart
            {
                Entries = timeRecords,
                ValueLabelOrientation = chartsOrientation,
                LabelOrientation = chartsOrientation,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.Gray
            };

            IsBusy = false;
        }

        public void UpdateTimeDomainChart()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.SettingUpChartsMessage;

            List<ChartEntry> timeRecords = new List<ChartEntry>();

            double minValue = sliderValue - numberOfShownSamplesFromTheMiddle > 0 ? sliderValue - numberOfShownSamplesFromTheMiddle : 0;
            double maxValue = sliderValue + numberOfShownSamplesFromTheMiddle < timeSamples.Count ? sliderValue + numberOfShownSamplesFromTheMiddle : timeSamples.Count;
            for (int i = (int)minValue; i < maxValue; i++)
            {
                timeRecords.Add(new ChartEntry((float)Math.Round(timeSamples[i], Constants.NumOfDecimalPlaces))
                {
                    Label = $"{i}.",
                    ValueLabel = $"{Math.Round(timeSamples[i], Constants.NumOfDecimalPlaces)}V",
                    Color = SkiaSharp.SKColor.Parse(Constants.TimeChartColor),
                    TextColor = SKColors.Gray,
                    ValueLabelColor = SKColors.Gray,
                });
            }

            TimeChart = new LineChart
            {
                Entries = timeRecords,
                ValueLabelOrientation = chartsOrientation,
                LabelOrientation = chartsOrientation,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.Gray
            };
            IsTimeChartVisible = true;
            if (areFreqSamplesReady) //because frequency is calcualted later
            {
                IsBusy = false;
            }
            else
            {
                BusyMessage = Resources.Strings.Resource.CalculateFFT;
            }
        }

        private void SetupFreqDomainChart()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.SettingUpFreqChartsMessage;
            List<ChartEntry> freqRecords = new List<ChartEntry>();

            int countToLoad = 2 * numberOfShownSamplesFromTheMiddle > freqSamples.Samples.Count() ? freqSamples.Samples.Count() : 2 * numberOfShownSamplesFromTheMiddle;
            for (int i = 0; i < countToLoad; i++)
            {
                freqRecords.Add(new ChartEntry((float)Math.Round(freqSamples.Samples[i].Sample, Constants.NumOfDecimalPlaces))
                {
                    Label = $"{Math.Round(freqSamples.Samples[i].Freq, Constants.NumOfDecimalPlaces)} Hz",
                    ValueLabel = $"{Math.Round(freqSamples.Samples[i].Sample, Constants.NumOfDecimalPlaces)}V",
                    Color = SkiaSharp.SKColor.Parse(Constants.FrequencyChartColor),
                    TextColor = SKColors.Gray,
                    ValueLabelColor = SKColors.Gray,
                });
            }

            FrequencyChart = new PointChart
            {
                Entries = freqRecords,
                ValueLabelOrientation = chartsOrientation,
                LabelOrientation = chartsOrientation,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.Gray
            };

            IsBusy = false;
        }

        public void UpdateFreqDomainChart()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.SettingUpFreqChartsMessage;
            List<ChartEntry> freqRecords = new List<ChartEntry>();

            double minValue = sliderValue - numberOfShownSamplesFromTheMiddle > 0 ? sliderValue - numberOfShownSamplesFromTheMiddle : 0;
            double maxValue = sliderValue + numberOfShownSamplesFromTheMiddle < freqSamples.Samples.Count() ? sliderValue + numberOfShownSamplesFromTheMiddle : freqSamples.Samples.Count();
            for (int i = (int)minValue; i < maxValue; i++)
            {
                freqRecords.Add(new ChartEntry((float)Math.Round(freqSamples.Samples[i].Sample, Constants.NumOfDecimalPlaces)) // todo change to fft
                {
                    Label = $"{Math.Round(freqSamples.Samples[i].Freq, Constants.NumOfDecimalPlaces)} Hz",
                    ValueLabel = $"{Math.Round(freqSamples.Samples[i].Sample, Constants.NumOfDecimalPlaces)}V",
                    Color = SkiaSharp.SKColor.Parse(Constants.FrequencyChartColor),
                    TextColor = SKColors.Gray,
                    ValueLabelColor = SKColors.Gray,
                });
            }

            FrequencyChart = new PointChart
            {
                Entries = freqRecords,
                ValueLabelOrientation = chartsOrientation,
                LabelOrientation = chartsOrientation,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.Gray
            };
            IsBusy = false;
        }

        private void UpdateCharts()
        {
            if (isTimeChartVisible)
            {
                UpdateTimeDomainChart();
            }

            if (isFreqChartVisible)
            {
                UpdateFreqDomainChart();
            }            
        }

        public async Task DelayedFreqChartLoad()
        {
            var shouldCalculateFftOnLoad = Preferences.Get(Constants.PrefsShouldCalculateFFT, false);
            if(shouldCalculateFftOnLoad)
            {
                await FreqChartLoad();
            }
        }

        private async Task FreqChartLoad()
        {
            IsBusy = true;
            IsExportButtonEnabled = false;
            await CalculateFFT();

            SetupFreqDomainChart();
            IsExportButtonEnabled = true;
            AreFreqSamplesReady = true;
            IsBusy = false;
        }

        private async Task CalculateFFT()
        {
            await Task.Run(() =>
            {
                BusyMessage = Resources.Strings.Resource.CalculateFFT;
                var samplingFreq = Preferences.Get(Constants.PrefsSavedSamplingFrequency,
                    Constants.MinSamplingFrequency);
                freqSamples = HelperFunctions.GenerateFreqSamples(timeSamples.ToArray(), samplingFreq);
            });
        }

        #endregion

        #region Excell handle
        private async Task ExportToExcel()
        {
            var fileName = $"{Constants.ExcellSheetName}-{Guid.NewGuid()}.xlsx";
            string filepath = excelService.PathToExcellFile(fileName);
            await Task.Run(() =>
            {
                IsBusy = true;
                IsExportButtonEnabled = false;

                ExcelStructure data = new ExcelStructure();
                
                if (timeSamples.Count > Constants.MaxEntriesForSheet)
                {
                    PopulateBigList(data, filepath, shouldExportTimeSamples);
                }
                else
                {
                    PopulateSmallList(data, filepath, Constants.ExcellSheetName1, shouldExportTimeSamples);
                }

                IsExportButtonEnabled = true;
                IsBusy = false;
            });


            await Launcher.OpenAsync(new OpenFileRequest()
            {
                File = new ReadOnlyFile(filepath)
            });
        }

        private void PopulateList(ExcelStructure data, bool shouldExportTimeDomain)
        {
            if(shouldExportTimeDomain)
            {
                string specifier = "G";
                CultureInfo culture = CultureInfo.CreateSpecificCulture("eu-ES");

                data.Headers = new List<string>()
                {
                    Resources.Strings.Resource.ID,
                    Resources.Strings.Resource.Sample
                };
                for (int i = 0; i < timeSamples.Count; i++)
                {
                    List<string> values = new List<string>
                    {
                        i.ToString(specifier, culture), timeSamples[i].ToString(specifier, culture)
                    };
                    data.Values.Add(values);

                    if (i % 10000 == 0)
                    {
                        BusyMessage = Resources.Strings.Resource.ExcellProcessing + $" {i}/{timeSamples.Count}";
                    }
                }
            }
            else
            {
                data.Headers = new List<string>()
                {
                    Resources.Strings.Resource.Freqz,
                    Resources.Strings.Resource.Sample
                };
                for (int i = 0; i < freqSamples.Samples.Count(); i++)
                {
                    List<string> values = new List<string>
                    {
                        freqSamples.Samples[i].FreqToString(), freqSamples.Samples[i].SampleToString()
                    };
                    data.Values.Add(values);

                    if (i % 10000 == 0)
                    {
                        BusyMessage = Resources.Strings.Resource.ExcellProcessing + $" {i}/{timeSamples.Count}";
                    }
                }
            }
            
            BusyMessage = Resources.Strings.Resource.ExportingToExcellMessage;
        }

        private void PopulateSmallList(ExcelStructure data, string filepath, string excellSheetName, bool shouldExportTimeDomain)
        {
            excelService.GenerateExcel(filepath);
            PopulateList(data, shouldExportTimeDomain);
            excelService.InsertDataIntoSheet(filepath, excellSheetName, data);
        }

        private void PopulateBigList(ExcelStructure data, string filepath, bool shouldExportTimeDomain)
        {
            PopulateList(data, shouldExportTimeDomain);
            excelService.CreateAndInsertDataToManySheets(filepath, data);
        }
        #endregion
        #endregion
    }
}
