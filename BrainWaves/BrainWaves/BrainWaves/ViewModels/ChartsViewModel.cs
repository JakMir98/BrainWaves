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
        #endregion

        #region Variables
        private Chart frequencyChart;
        private Chart timeChart;
        private List<float> samples = new List<float>();
        private ExcelService excelService;
        private bool isFreqChartVisible = false;
        private bool isTimeChartVisible = true;
        private bool isExportButtonEnabled = true;
        private double minSliderValue;
        private double maxSliderValue;
        private double sliderValue;
        private string text;
        private int numberOfShownSamplesFromTheMiddle;
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

            Random random = new Random();
            double range = 3.3;
            for (int i = 0; i < MinSampleNum; i++)
            {
                double sample = random.NextDouble();
                double scaled = (sample * range);
                float f = (float)scaled;
                samples.Add((float)f);
            }
            Text = $"count = {samples.Count} value[0]={samples[0]}";

            //after samples
            MinSliderValue = 0;
            MaxSliderValue = samples.Count;
            numberOfShownSamplesFromTheMiddle = Preferences.Get(Constants.PrefsSamplesToShowFromMiddle, DefaultLoadedSamples);

            SetupTimeDomainChart();
            SetupFreqDomainChart();
        }

        public ChartsViewModel(List<float> _samples)
        {
            samples = _samples;

            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            numberOfShownSamplesFromTheMiddle = Preferences.Get(Constants.PrefsSamplesToShowFromMiddle, DefaultLoadedSamples);

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            DragCompletedCommand = new Command(UpdateCharts);

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
            }
        }
        #endregion

        #region Functions
        private void SetupTimeDomainChart()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.SettingUpChartsMessage;
            List<ChartEntry> timeRecords = new List<ChartEntry>();
            
            int countToLoad = 2 * numberOfShownSamplesFromTheMiddle > samples.Count ? samples.Count : 2 * numberOfShownSamplesFromTheMiddle;
            for (int i = 0; i < countToLoad; i++)
            {
                timeRecords.Add(new ChartEntry(samples[i])
                {
                    Label = $"{i}.",
                    ValueLabel = $"{samples[i]}V",
                    Color = SkiaSharp.SKColor.Parse(TimeColor),
                    TextColor = SKColors.White,
                    ValueLabelColor = SKColors.White,
                });
            }

            TimeChart = new PointChart
            {
                Entries = timeRecords,
                ValueLabelOrientation = Orientation.Vertical,
                LabelOrientation = Orientation.Vertical,
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
            double maxValue = sliderValue + numberOfShownSamplesFromTheMiddle < samples.Count ? sliderValue + numberOfShownSamplesFromTheMiddle : samples.Count;
            for (int i = (int)minValue; i < maxValue; i++)
            {
                timeRecords.Add(new ChartEntry(samples[i])
                {
                    Label = $"{i}.",
                    ValueLabel = $"{samples[i]}V",
                    Color = SkiaSharp.SKColor.Parse(TimeColor),
                    TextColor = SKColors.White,
                    ValueLabelColor = SKColors.White,
                });
            }

            TimeChart = new PointChart
            {
                Entries = timeRecords,
                ValueLabelOrientation = Orientation.Vertical,
                LabelOrientation = Orientation.Vertical,
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

            int countToLoad = 2 * numberOfShownSamplesFromTheMiddle > samples.Count ? samples.Count : 2 * numberOfShownSamplesFromTheMiddle;
            for (int i = 0; i < countToLoad; i++) // todo change to fft
            {
                freqRecords.Add(new ChartEntry(samples[i])
                {
                    Label = $"{i} Hz",
                    ValueLabel = $"{samples[i]}V",
                    Color = SkiaSharp.SKColor.Parse(FrequencyColor),
                    TextColor = SKColors.White,
                    ValueLabelColor = SKColors.White,
                });
            }

            FrequencyChart = new PointChart
            {
                Entries = freqRecords,
                ValueLabelOrientation = Orientation.Vertical,
                LabelOrientation = Orientation.Vertical,
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
            double maxValue = sliderValue + numberOfShownSamplesFromTheMiddle < samples.Count ? sliderValue + numberOfShownSamplesFromTheMiddle : samples.Count;
            for (int i = (int)minValue; i < maxValue; i++)
            {
                freqRecords.Add(new ChartEntry(samples[i]) // todo change to fft
                {
                    Label = $"{i} Hz",
                    ValueLabel = $"{samples[i]}V",
                    Color = SkiaSharp.SKColor.Parse(FrequencyColor),
                    TextColor = SKColors.White,
                    ValueLabelColor = SKColors.White,
                });
            }

            FrequencyChart = new PointChart
            {
                Entries = freqRecords,
                ValueLabelOrientation = Orientation.Vertical,
                LabelOrientation = Orientation.Vertical,
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

        #region Excell handle
        private async Task ExportToExcel()
        {
            var fileName = $"{Constants.ExcellSheetName}-{Guid.NewGuid()}.xlsx";
            string filepath = excelService.PathToExcellFile(fileName);
            await Task.Run(() =>
            {
                IsBusy = true;
                IsExportButtonEnabled = false;

                var data = new ExcelStructure
                {
                    Headers = new List<string>()
                    {
                        Resources.Strings.Resource.ID,
                        Resources.Strings.Resource.Sample
                    }
                };

                if (samples.Count > Constants.MaxEntriesForSheet)
                {
                    PopulateBigList(data, filepath);
                }
                else
                {
                    PopulateSmallList(data, filepath, Constants.ExcellSheetName1);
                }

                IsExportButtonEnabled = true;
                IsBusy = false;
            });


            await Launcher.OpenAsync(new OpenFileRequest()
            {
                File = new ReadOnlyFile(filepath)
            });
        }

        private void PopulateList(ExcelStructure data)
        {
            for (int i = 0; i < samples.Count; i++)
            {
                List<string> values = new List<string>
                {
                    i.ToString(), samples[i].ToString()
                };
                data.Values.Add(values);

                if (i % 10000 == 0)
                {
                    BusyMessage = Resources.Strings.Resource.ExcellProcessing + $" {i}/{samples.Count}";
                }
            }
            BusyMessage = Resources.Strings.Resource.ExportingToExcellMessage;
        }

        private void PopulateSmallList(ExcelStructure data, string filepath, string excellSheetName)
        {
            excelService.GenerateExcel(filepath);
            PopulateList(data);
            excelService.InsertDataIntoSheet(filepath, excellSheetName, data);
        }

        private void PopulateBigList(ExcelStructure data, string filepath)
        {
            PopulateList(data);
            excelService.CreateAndInsertDataToManySheets(filepath, data);
        }
        #endregion
        
        #endregion
    }
}
