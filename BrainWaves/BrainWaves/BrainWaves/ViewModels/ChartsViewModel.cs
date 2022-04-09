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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using FftSharp;

namespace BrainWaves.ViewModels
{
    public class ChartsViewModel : BaseViewModel
    {
        private const string FrequencyColor = "#FF1493";
        private const string TimeColor = "#00BFFF";

        private Chart frequencyChart;
        private Chart timeChart;
        private List<float> samples = new List<float>();
        private ExcelService excelService;
        private bool isFreqChartVisible = false;
        private bool isTimeChartVisible = false;

        private PlotModel timePlotModel;
        private PlotModel freqPlotModel;

        public ICommand ExportToExcelCommand { private set; get; }
        public ICommand CreateCommand { private set; get; }

        private string text;

        public ChartsViewModel()
        {
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            CreateCommand = new Command(Create);
            for(int i = 0; i < 4096; i++)
            {
                samples.Add((float)i);
            }
            Create();
        }

        public ChartsViewModel(List<float> _samples)
        {
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            
            samples = _samples;
            Create();
        }

        public async Task SetupChartsAsync()
        {
            //await SetupCharts();
        }

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

        public PlotModel TimePlotModel
        {
            get => timePlotModel;
            set => SetProperty(ref timePlotModel, value);
        }

        public PlotModel FreqPlotModel
        {
            get => freqPlotModel;
            set => SetProperty(ref freqPlotModel, value);
        }

        private Task SetupCharts()
        {
            var t = Task.Run(() =>
            {
                IsBusy = true;
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
                if(App.fSamples.Count > 0)
                {
                    Text = $"num of samples = {App.fSamples.Count}\n max value = {App.fSamples.Max()}\n min value = {App.fSamples.Min()}";
                }
                IsBusy = false;
                
            });
            return t;
        }

        private async Task ExportToExcel()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.ExportingToExcellMessage;
            var fileName = $"{Constants.ExcellSheetName}-{Guid.NewGuid()}.xlsx";
            string filepath = excelService.GenerateExcel(fileName);

            var data = new ExcelStructure
            {
                Headers = new List<string>()
                {
                    Resources.Strings.Resource.ID,
                    Resources.Strings.Resource.Sample
                }
            };

            for(int i = 0; i < App.fSamples.Count; i++)
            {
                List<string> values = new List<string>
                {
                    i.ToString(), App.fSamples[i].ToString()
                };
                data.Values.Add(values);
            }

            excelService.InsertDataIntoSheet(filepath, Constants.ExcellSheetName, data);

            await Launcher.OpenAsync(new OpenFileRequest()
            {
                File = new ReadOnlyFile(filepath)
            });
            
            IsBusy = false;
        }

        public void Create()
        {
            // https://en.wikipedia.org/wiki/Normal_distribution
            TimePlotModel = new PlotModel
            {
                Title = "Time plot",
                Subtitle = "Sines"
            };

            TimePlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -3.5,
                Maximum = 3.5,
                MajorStep = 0.5,
                MinorStep = 0.05,
                TickStyle = TickStyle.Inside
            });
            TimePlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 600,
                MajorStep = 50,
                MinorStep = 10,
                TickStyle = TickStyle.Inside
            });
            TimePlotModel.Series.Add(CreateSine());
            TimePlotModel.Series.Add(CreatSineFiltered());
            TimePlotModel.Series.Add(CreatSineWindowed());


            FreqPlotModel = new PlotModel
            {
                Title = "FFT",
                Subtitle = "Fourier transform"
            };

            FreqPlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -10,
                Maximum = 300,
                MajorStep = 50,
                MinorStep = 25,
                TickStyle = TickStyle.Inside
            });
            FreqPlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 24000,
                MajorStep = 1000,
                MinorStep = 500,
                TickStyle = TickStyle.Inside
            });
            FreqPlotModel.Series.Add(CreatFFTSignal());
        }

        private LineSeries CreateNormalDistributionSeries(double x0, double x1, double mean, double variance, int n = 1000)
        {
            var ls = new LineSeries
            {
                Title = string.Format("μ={0}, σ²={1}", mean, variance)
            };

            for (int i = 0; i < n; i++)
            {
                double x = x0 + ((x1 - x0) * i / (n - 1));
                double f = 1.0 / Math.Sqrt(2 * Math.PI * variance) * Math.Exp(-(x - mean) * (x - mean) / 2 / variance);
                ls.Points.Add(new DataPoint(x, f));
            }

            return ls;
        }

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

        private double[] GenerateSinWave(int sampleFreq, int numOfSamples, float amplitude)
        {
            double[] sinWave = new double[numOfSamples];
            for (int i = 0; i < numOfSamples; i++)
            {
                sinWave[i] = amplitude * Math.Sin(i);
            }
            return sinWave;
        }
    }
}
