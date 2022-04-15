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
        private const string FrequencyColor = "#FF1493";
        private const string TimeColor = "#00BFFF";
        private const int DefaultLoadedSamples = 20;
        private const int MaxLoadedSamples = 500;
        private const int NumOfDecimalPlaces = 2;
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
        private string text;
        private int numberOfShownSamplesFromTheMiddle;
        private FrequencySamplesContainer freqSamples;
        private Orientation chartsOrientation;
        private bool shouldExportTimeSamples = true;
        #endregion

        #region ICommands
        public ICommand ExportToExcelCommand { private set; get; }
        public ICommand DragCompletedCommand { private set; get; }
        #endregion

        #region Constructors
        public ChartsViewModel()
        {
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            DragCompletedCommand = new Command(UpdateCharts);

            // Create samples to test Export to excell
            int samplingFreq = 500;
            int length = 256;
            double[] sinWave = GenerateSinWave(samplingFreq, length, 1, 50);
            double[] sinWavePadded = FftSharp.Pad.ZeroPad(sinWave);
            timeSamples = new List<double>(sinWavePadded);
            freqSamples = new FrequencySamplesContainer(Transform.Absolute(Transform.FFT(sinWavePadded)),
                            Transform.FFTfreq(samplingFreq, length));
            /*
            Random random = new Random();
            double range = 3.3;
            for (int i = 0; i < MinSampleNum; i++)
            {
                double sample = random.NextDouble();
                double scaled = (sample * range);
                float f = (float)scaled;
                samples.Add((float)f);
            }
            */
            Text = $"count = {timeSamples.Count}\n";
         

            //after samples
            MinSliderValue = 0;
            MaxSliderValue = timeSamples.Count;
            numberOfShownSamplesFromTheMiddle = Preferences.Get(Constants.PrefsSamplesToShowFromMiddle, DefaultLoadedSamples);
            CheckChartLabelOrientation();
            SetupTimeDomainChart();
            SetupFreqDomainChart();
        }

        public ChartsViewModel(List<double> _samples)
        {
            timeSamples = _samples;

            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            numberOfShownSamplesFromTheMiddle = Preferences.Get(Constants.PrefsSamplesToShowFromMiddle, DefaultLoadedSamples);

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            DragCompletedCommand = new Command(UpdateCharts);

            CheckChartLabelOrientation();
            SetupTimeDomainChart();
            SetupFreqDomainChart();
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

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
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
                if(value > MaxLoadedSamples)
                {
                    tempValue = MaxLoadedSamples;
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
        #endregion

        #region Functions
        #region Charts Handle
        private void CheckChartLabelOrientation()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                if (numberOfShownSamplesFromTheMiddle > DefaultLoadedSamples - 15)
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
                if (numberOfShownSamplesFromTheMiddle > DefaultLoadedSamples - 10)
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
                timeRecords.Add(new ChartEntry((float)Math.Round(timeSamples[i], NumOfDecimalPlaces))
                {
                    Label = $"{i}.",
                    ValueLabel = $"{Math.Round(timeSamples[i], NumOfDecimalPlaces)}V",
                    Color = SkiaSharp.SKColor.Parse(TimeColor),
                    TextColor = SKColors.White,
                    ValueLabelColor = SKColors.White,
                });
            }

            TimeChart = new LineChart
            {
                Entries = timeRecords,
                ValueLabelOrientation = chartsOrientation,
                LabelOrientation = chartsOrientation,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.White
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
                timeRecords.Add(new ChartEntry((float)Math.Round(timeSamples[i], NumOfDecimalPlaces))
                {
                    Label = $"{i}.",
                    ValueLabel = $"{Math.Round(timeSamples[i], NumOfDecimalPlaces)}V",
                    Color = SkiaSharp.SKColor.Parse(TimeColor),
                    TextColor = SKColors.White,
                    ValueLabelColor = SKColors.White,
                });
            }

            TimeChart = new LineChart
            {
                Entries = timeRecords,
                ValueLabelOrientation = chartsOrientation,
                LabelOrientation = chartsOrientation,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.White
            };
            IsTimeChartVisible = true;
            IsBusy = false;
        }

        private void SetupFreqDomainChart()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.SettingUpChartsMessage;
            List<ChartEntry> freqRecords = new List<ChartEntry>();

            int countToLoad = 2 * numberOfShownSamplesFromTheMiddle > freqSamples.Samples.Count() ? freqSamples.Samples.Count() : 2 * numberOfShownSamplesFromTheMiddle;
            for (int i = 0; i < countToLoad; i++) // todo change to fft
            {
                freqRecords.Add(new ChartEntry((float)Math.Round(freqSamples.Samples[i].Sample, NumOfDecimalPlaces))
                {
                    Label = $"{Math.Round(freqSamples.Samples[i].Freq, NumOfDecimalPlaces)} Hz",
                    ValueLabel = $"{Math.Round(freqSamples.Samples[i].Sample, NumOfDecimalPlaces)}V",
                    Color = SkiaSharp.SKColor.Parse(FrequencyColor),
                    TextColor = SKColors.White,
                    ValueLabelColor = SKColors.White,
                });
            }

            FrequencyChart = new PointChart
            {
                Entries = freqRecords,
                ValueLabelOrientation = chartsOrientation,
                LabelOrientation = chartsOrientation,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.White
            };

            IsBusy = false;
        }

        public void UpdateFreqDomainChart()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.SettingUpChartsMessage;
            List<ChartEntry> freqRecords = new List<ChartEntry>();

            double minValue = sliderValue - numberOfShownSamplesFromTheMiddle > 0 ? sliderValue - numberOfShownSamplesFromTheMiddle : 0;
            double maxValue = sliderValue + numberOfShownSamplesFromTheMiddle < freqSamples.Samples.Count() ? sliderValue + numberOfShownSamplesFromTheMiddle : freqSamples.Samples.Count();
            for (int i = (int)minValue; i < maxValue; i++)
            {
                freqRecords.Add(new ChartEntry((float)Math.Round(freqSamples.Samples[i].Sample, NumOfDecimalPlaces)) // todo change to fft
                {
                    Label = $"{Math.Round(freqSamples.Samples[i].Freq, NumOfDecimalPlaces)} Hz",
                    ValueLabel = $"{Math.Round(freqSamples.Samples[i].Sample, NumOfDecimalPlaces)}V",
                    Color = SkiaSharp.SKColor.Parse(FrequencyColor),
                    TextColor = SKColors.White,
                    ValueLabelColor = SKColors.White,
                });
            }

            FrequencyChart = new PointChart
            {
                Entries = freqRecords,
                ValueLabelOrientation = chartsOrientation,
                LabelOrientation = chartsOrientation,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.White
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

        private double[] GenerateSinWave(int samplingFrequency, int length, float amplitude, int signalFrequency)
        {
            float T = (float)1 / samplingFrequency;            // % Sampling period
            float[] t = new float[length];
            for (int i = 0; i < length - 1; i++)
            {
                t[i] = i * T;
            }

            double[] sinWave = new double[length];
            for (int i = 0; i < length; i++)
            {
                sinWave[i] = amplitude * Math.Sin(2 * Math.PI * signalFrequency * t[i]);
            }
            return sinWave;
        }

        private bool IsPowerOfTwo(ulong x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }
        #endregion
    }
}
