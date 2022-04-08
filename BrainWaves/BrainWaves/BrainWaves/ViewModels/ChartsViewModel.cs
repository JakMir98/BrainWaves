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

        public ICommand ExportToExcelCommand { private set; get; }

        private string text;

        public ChartsViewModel()
        {
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
        }

        public ChartsViewModel(List<float> _samples)
        {
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            
            samples = _samples;
        }

        public async Task SetupChartsAsync()
        {
            await SetupCharts();
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

        private async Task SetupCharts()
        {
            var t = Task.Run(async () =>
            {
                IsBusy = true;
                BusyMessage = Resources.Strings.Resource.SettingUpCharts;
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
        }

        private async Task ExportToExcel()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.ExportingToExcellText;
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
    }
}
