﻿using BrainWaves.Helpers;
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

        public ICommand ExportToExcelCommand { private set; get; }

        private string text;
        
        public ChartsViewModel(List<float> _samples)
        {
            Title = Resources.Strings.Resource.Charts;
            excelService = new ExcelService();

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            GoBackCommand = new Command(async () => await GoBack());
            
            samples = _samples;

            SetupCharts();
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

        private void SetupCharts()
        {
            List<ChartEntry> frequencyRecords = new List<ChartEntry>();
            
            List<ChartEntry> timeRecords = new List<ChartEntry>();
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
            /*
            FrequencyChart = new LineChart
            {
                Entries = frequencyRecords,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.White
            };
            */
            TimeChart = new PointChart
            {
                Entries = timeRecords,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColors.White
            };

            Text = $"num of samples = {timeRecords.Count}";
            
        }

        private async Task ExportToExcel()
        {
            IsBusy = true;
            
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

            /*
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
            */
            excelService.InsertDataIntoSheet(filepath, Constants.ExcellSheetName, data);

            await Launcher.OpenAsync(new OpenFileRequest()
            {
                File = new ReadOnlyFile(filepath)
            });
            
            IsBusy = false;
        }
    }
}
