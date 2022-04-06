using Microcharts;
using SkiaSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrainWaves.ViewModels
{
    public class ChartsViewModel : BaseViewModel
    {
        private Chart frequencyChart;

        public ICommand ExportToExcelCommand { private set; get; }
        
        public ChartsViewModel()
        {
            Title = Resources.Strings.Resource.Charts;
            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            SetupCharts();
        }

        public Chart FrequencyChart
        {
            get => frequencyChart;
            set => SetProperty(ref frequencyChart, value);
        }

        private void SetupCharts()
        {
            List<ChartEntry> frequencyRecords = new List<ChartEntry>();
            /*
            foreach (var item in GetChosenRecords())
            {
                humidityRecords.Add(new ChartEntry(item.Humidity)
                {
                    Label = $"{item.ID}.",
                    ValueLabel = $"{item.Humidity}%",
                    Color = SkiaSharp.SKColor.Parse(HumidityColor),
                    TextColor = SKColors.White,
                    ValueLabelColor = SKColors.White,
                });
            }
            */

            FrequencyChart = new LineChart
            {
                Entries = frequencyRecords,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.White
            };
        }

        private async Task ExportToExcel()
        {
            IsBusy = true;
            /*
            var fileName = $"{Constants.RecordServerClassName}-{Guid.NewGuid()}.xlsx";
            string filepath = excelService.GenerateExcel(fileName);

            var data = new ExcelStructure
            {
                Headers = new List<string>()
                {
                    Resources.Strings.Resource.ID,
                    Resources.Strings.Resource.TimeStamp,
                    Resources.Strings.Resource.Temperature,
                    Resources.Strings.Resource.Humidity,
                    Resources.Strings.Resource.SoilMoisture1,
                    Resources.Strings.Resource.SoilMoisture2,
                    Resources.Strings.Resource.SoilMoisture3,
                    Resources.Strings.Resource.SoilMoisture4,
                    Resources.Strings.Resource.SoilMoisture5,
                    Resources.Strings.Resource.SoilMoisture6,
                    Resources.Strings.Resource.SoilMoisture7,
                    Resources.Strings.Resource.SoilMoisture8,
                    Resources.Strings.Resource.SoilMoisture9,
                    Resources.Strings.Resource.SoilMoisture10
                }
            };

            foreach (var item in GetChosenRecords())
            {
                List<string> additems = new List<string>
                {
                    item.ID.ToString(),
                    item.TimeStamp.ToString(),
                    item.Temperature.ToString(),
                    item.Humidity.ToString()
                };
                foreach (var soilInItem in item.SoilMoisture)
                {
                    additems.Add(soilInItem.ToString());
                }

                data.Values.Add(additems);
            }

            excelService.InsertDataIntoSheet(filepath, Constants.RecordServerClassName, data);

            await Launcher.OpenAsync(new OpenFileRequest()
            {
                File = new ReadOnlyFile(filepath)
            });
            */
            IsBusy = false;
        }
    }
}
