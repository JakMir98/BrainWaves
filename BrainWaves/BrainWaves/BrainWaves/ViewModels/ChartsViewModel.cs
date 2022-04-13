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
        #endregion

        #region Variables
        private Chart frequencyChart;
        private Chart timeChart;
        private List<float> samples = new List<float>();
        private ExcelService excelService;
        private bool isFreqChartVisible = false;
        private bool isTimeChartVisible = false;
        private bool isExportButtonEnabled = true;
        private bool isCreateButtonEnabled = true;

        private string text;
        #endregion

        #region ICommands
        public ICommand ExportToExcelCommand { private set; get; }
        public ICommand CreateCommand { private set; get; }
        #endregion

        #region Constructors
        public ChartsViewModel()
        {
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            CreateCommand = new Command(async () => await SetupCharts());

            Random random = new Random();
            double range = 3.3;
            for (int i = 0; i < MaxSampleNum; i++)
            {
                double sample = random.NextDouble();
                double scaled = (sample * range);
                float f = (float)scaled;
                samples.Add((float)f);
            }
            IsTimeChartVisible = true;
            Text = $"count = {samples.Count} value[0]={samples[0]}";
            //Create();
        }

        public ChartsViewModel(List<float> _samples)
        {
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            
            samples = _samples;
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

        public bool IsCreateButtonEnabled
        {
            get => isCreateButtonEnabled;
            set => SetProperty(ref isCreateButtonEnabled, value);
        }
        #endregion

        #region Functions
        public async Task SetupChartsAsync()
        {
            //await SetupCharts();
        }

        private async Task SetupCharts()
        {
            await Task.Run(() =>
            {
                IsBusy = true;
                IsCreateButtonEnabled = false;
                BusyMessage = Resources.Strings.Resource.SettingUpChartsMessage;
                List<ChartEntry> frequencyRecords = new List<ChartEntry>();

                List<ChartEntry> timeRecords = new List<ChartEntry>();

                //foreach (var item in App.fSamples)
                foreach (var item in samples)
                {
                    timeRecords.Add(new ChartEntry(item)
                    {
                        Label = $".",
                        ValueLabel = $"{item}V",
                        Color = SkiaSharp.SKColor.Parse(TimeColor),
                        TextColor = SKColors.White,
                        ValueLabelColor = SKColors.White,
                    });
                }
               
                FrequencyChart = new LineChart
                {
                    Entries = frequencyRecords,
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelOrientation = Orientation.Horizontal,
                    BackgroundColor = SKColors.Transparent,
                    LabelColor = SKColors.White
                };
               
                TimeChart = new PointChart
                {
                    Entries = timeRecords,
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelOrientation = Orientation.Horizontal,
                    BackgroundColor = SKColors.Transparent,
                    LabelColor = SKColors.White
                };
                  /**/
                IsFreqChartVisible = true;
                IsTimeChartVisible = true;
                IsCreateButtonEnabled = true;
                IsBusy = false;
            });
        }

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

        /*
        private LineSeries CreateSine()
        {
            var ls = new LineSeries
            {
                Title = string.Format("Sine")
            };
            double[] signal = GenerateSinWave(250,1024, 2);//FftSharp.SampleData.SampleAudio1();

            int counter = 0;
            foreach (var item in signal)
            {
                ls.Points.Add(new DataPoint(counter++, item));
            }

            return ls;
        }

        private LineSeries CreatSineFiltered()
        {
            var ls = new LineSeries
            {
                Title = string.Format("Sine filtered")
            };
            int sampleRate = 48_000;
            double[] signal = FftSharp.SampleData.SampleAudio1();
            double[] filtered = FftSharp.Filter.LowPass(signal, sampleRate, maxFrequency: 2000);

            int counter = 0;
            foreach(var item in filtered)
            {
                ls.Points.Add(new DataPoint(counter++, item));
            }

            return ls;
        }

        private LineSeries CreatSineWindowed()
        {
            var ls = new LineSeries()
            {
                Title = "Sine windowed"
            };
            double[] signal = FftSharp.SampleData.SampleAudio1();
            var window = new FftSharp.Windows.Hanning();
            double[] windowed = window.Apply(signal);
            int counter = 0;
            foreach (var item in windowed)
            {
                ls.Points.Add(new DataPoint(counter++, item));
            }

            return ls;
        }

        private LineSeries CreatFFTSignal()
        {
            var ls = new LineSeries()
            {
                Title = "FFT"
            };
            

            int sampleRate = 48_000;
            double[] signal = GenerateSinWave(250, 1024, 2);//FftSharp.SampleData.SampleAudio1();
            var window = new FftSharp.Windows.Hanning();
            window.ApplyInPlace(signal);
            Complex[] fftRaw = FftSharp.Transform.FFT(signal);
            double[] freq = FftSharp.Transform.FFTfreq(sampleRate, fftRaw.Length);
            int counter = 0;
            foreach (var item in fftRaw)
            {
                ls.Points.Add(new DataPoint(freq[counter++], item.Magnitude));
            }

            return ls;
        }
        */
        private double[] GenerateSinWave(int sampleFreq, int numOfSamples, float amplitude)
        {
            double[] sinWave = new double[numOfSamples];
            for (int i = 0; i < numOfSamples; i++)
            {
                sinWave[i] = amplitude * Math.Sin(i);
            }
            return sinWave;
        }
        #endregion
    }
}
