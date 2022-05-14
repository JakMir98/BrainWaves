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
using System.Globalization;
using System.Text;

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
        private List<double> backupTimeSamples = new List<double>();
        private ExcelService excelService;
        private bool isFreqChartVisible = false;
        private bool isTimeChartVisible = true;
        private bool isExportButtonEnabled = true;
        private double minSliderValue;
        private double maxSliderValue;
        private double sliderValue;
        private int numberOfShownSamplesFromTheMiddle;
        private List<Sample> freqSamples;
        private Orientation chartsOrientation;
        private bool shouldExportTimeSamples = true;
        private bool areFreqSamplesReady = false;
        private int samplingFrequency;
        private string selectedFFTSettings;
        private List<string> availableFFTSettings;
        private bool areOriginalSamples = true;
        #endregion

        #region ICommands
        public ICommand ExportToExcelCommand { private set; get; }
        public ICommand ExportToCSVCommand { private set; get; }
        public ICommand DragCompletedCommand { private set; get; }
        public ICommand CalculateFFTCommand { private set; get; }
        public ICommand GoToSettingsCommand { private set; get; }
        #endregion

        #region Constructors
        public ChartsViewModel()
        {
            BasicAndCommandsInit();

            /* Create samples to test Export to excell */
            int samplingFreq = 500;
            int length = 1048576; //2^20
            double[] sinWave = TestSamplesGenerator.GenerateSinWave(samplingFreq, length, 1, 50);
            timeSamples = new List<double>(sinWave);
            freqSamples = HelperFunctions.GenerateFreqSamples(sinWave, samplingFreq);

            /* Generate random values
            //timeSamples = new List<double>(HelperFunctions.GenerateRandomValues(MinSampleNum));
            */

            InitChartsAndSlider();
            SetupFreqDomainChart();
        }

        public ChartsViewModel(List<double> _samples) // when samples from esp32
        {
            timeSamples = _samples;
            freqSamples = new List<Sample>();
            BasicAndCommandsInit();
            InitChartsAndSlider();
        }

        public ChartsViewModel(List<double> _samples, int fs) // when sinwave created
        {
            timeSamples = _samples;
            freqSamples = new List<Sample>();
            BasicAndCommandsInit();
            samplingFrequency = fs;
            InitChartsAndSlider();
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
                SetProperty(ref numberOfShownSamplesFromTheMiddle, value);
                Preferences.Set(Constants.PrefsSamplesToShowFromMiddle, value);
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

        public string SelectedFFTSettings
        {
            get => selectedFFTSettings;
            set
            {
                SetProperty(ref selectedFFTSettings, value);
                SelectedFFTSettingsChangedHandle(value);
            }
        }
        public List<string> AvailableFFTSettings
        {
            get => availableFFTSettings;
            set => SetProperty(ref availableFFTSettings, value);
        }
        #endregion

        #region Functions
        private void BasicAndCommandsInit()
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
            samplingFrequency = Preferences.Get(Constants.PrefsSavedSamplingFrequency,
                    Constants.MinSamplingFrequency);
            AvailableFFTSettings = new List<string>
            {
                Resources.Strings.Resource.FFTSettingsDefault,
                Resources.Strings.Resource.FFTSettingsFilter,
                Resources.Strings.Resource.FFTSettingsWindow,
                Resources.Strings.Resource.FFTSettingsFilterAndWindow
            };
            selectedFFTSettings = Resources.Strings.Resource.FFTSettingsDefault;

            ExportToExcelCommand = new Command(async () => await ExportToExcel());
            ExportToCSVCommand = new Command(async () => await ExportToCSV());
            GoBackCommand = new Command(async () => await GoBack());
            DragCompletedCommand = new Command(UpdateCharts);
            CalculateFFTCommand = new Command(async () => await FreqChartLoad());
            GoToSettingsCommand = new Command(async () => await GoToSettings());
        }

        private void InitChartsAndSlider()
        {
            MinSliderValue = 0;
            MaxSliderValue = timeSamples.Count;
            numberOfShownSamplesFromTheMiddle = Preferences.Get(Constants.PrefsSamplesToShowFromMiddle, Constants.DefaultLoadedSamples);
            CheckChartLabelOrientation();
            SetupTimeDomainChart();
        }

        private void SelectedFFTSettingsChangedHandle(string value)
        {
            if (value == Resources.Strings.Resource.FFTSettingsDefault)
            {
                RevertSamples();
            }
            else if (value == Resources.Strings.Resource.FFTSettingsWindow)
            {
                WindowSamples();
            }
            else if (value == Resources.Strings.Resource.FFTSettingsFilter)
            {
                FilterSamples();
            }
            else if (value == Resources.Strings.Resource.FFTSettingsFilterAndWindow)
            {
                WindowAndFilterSamples();
            }
            SliderValue = 0;
        }       

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
                timeRecords.Add(HelperFunctions.GenerateChartEntryForTimeSamples(timeSamples[i], i, Constants.Colors[0]));
            }

            TimeChart = HelperFunctions.GenerateLineChart(timeRecords, chartsOrientation);

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
                timeRecords.Add(HelperFunctions.GenerateChartEntryForTimeSamples(timeSamples[i], i, Constants.Colors[0]));
            }

            TimeChart = HelperFunctions.GenerateLineChart(timeRecords, chartsOrientation);

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

            int countToLoad = 2 * numberOfShownSamplesFromTheMiddle > freqSamples.Count() ? freqSamples.Count() : 2 * numberOfShownSamplesFromTheMiddle;
            for (int i = 0; i < countToLoad; i++)
            {
                freqRecords.Add(HelperFunctions.GenerateChartEntryForFreqSamples(freqSamples[i], Constants.Colors[1]));
            }

            FrequencyChart = HelperFunctions.GeneratePointChart(freqRecords, chartsOrientation);

            IsBusy = false;
        }

        public void UpdateFreqDomainChart()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.SettingUpFreqChartsMessage;
            List<ChartEntry> freqRecords = new List<ChartEntry>();

            if(freqSamples != null)
            {
                double minValue = sliderValue - numberOfShownSamplesFromTheMiddle > 0 ? sliderValue - numberOfShownSamplesFromTheMiddle : 0;
                double maxValue = sliderValue + numberOfShownSamplesFromTheMiddle < freqSamples.Count() ? sliderValue + numberOfShownSamplesFromTheMiddle : freqSamples.Count();
                for (int i = (int)minValue; i < maxValue; i++)
                {
                    freqRecords.Add(HelperFunctions.GenerateChartEntryForFreqSamples(freqSamples[i], Constants.Colors[1]));
                }

                FrequencyChart = HelperFunctions.GeneratePointChart(freqRecords, chartsOrientation);
            }
            
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
                freqSamples = HelperFunctions.GenerateFreqSamples(timeSamples.ToArray(), samplingFrequency);
            });
        }
        
        private void FilterSamples()
        {
            Task.Run(() =>
            {
                IsBusy = true;
                if (areOriginalSamples)
                {
                    backupTimeSamples = new List<double>(timeSamples);
                }
                else
                {
                    timeSamples = new List<double>(backupTimeSamples);
                }
                areOriginalSamples = false;
                double[] fValues = timeSamples.ToArray();
                HelperFunctions.PerformZeroPaddingIfNeeded(ref fValues);
                var cutoffFreq = Preferences.Get(Constants.PrefsCutoffFreqOfLowPassFilter, Constants.DefaultLowPassFilterMaxFreq);
                fValues = FftSharp.Filter.LowPass(fValues, samplingFrequency, cutoffFreq);
                timeSamples = new List<double>(fValues);
                IsBusy = false;
            });

            SetupTimeDomainChart();
        }
        
        private void WindowSamples()
        {
            Task.Run(() =>
            {
                IsBusy = true;
                if(areOriginalSamples)
                {
                    backupTimeSamples = new List<double>(timeSamples);
                }
                else
                {
                    timeSamples = new List<double>(backupTimeSamples);
                }
                areOriginalSamples = false;

                double[] fValues = timeSamples.ToArray();
                HelperFunctions.PerformZeroPaddingIfNeeded(ref fValues);
                var window = new FftSharp.Windows.Hanning();
                window.ApplyInPlace(fValues);
                timeSamples = new List<double>(fValues);
                IsBusy = false;
            });

            SetupTimeDomainChart();
        }
        
        private void WindowAndFilterSamples()
        {
            Task.Run(() =>
            {
                IsBusy = true;
                if (areOriginalSamples)
                {
                    backupTimeSamples = new List<double>(timeSamples);
                }
                else
                {
                    timeSamples = new List<double>(backupTimeSamples);
                }
                areOriginalSamples = false;
                double[] fValues = timeSamples.ToArray();
                HelperFunctions.PerformZeroPaddingIfNeeded(ref fValues);
                var window = new FftSharp.Windows.Hanning();
                window.ApplyInPlace(fValues);
                var cutoffFreq = Preferences.Get(Constants.PrefsCutoffFreqOfLowPassFilter, Constants.DefaultLowPassFilterMaxFreq);
                fValues = FftSharp.Filter.LowPass(fValues, samplingFrequency, cutoffFreq);
                timeSamples = new List<double>(fValues);
                IsBusy = false;
            });

            SetupTimeDomainChart();
        }

        private void RevertSamples()
        {
            Task.Run(() =>
            {
                IsBusy = true;
                if(backupTimeSamples.Count > 0)
                {
                    timeSamples = new List<double>(backupTimeSamples);
                    backupTimeSamples.Clear();
                    areOriginalSamples = true;
                }
                IsBusy = false;
            });

            SetupTimeDomainChart();
        }
        #endregion

        #region Excel handle
        private async Task ExportToExcel()
        {
            var fileName = $"{Constants.ExcellSheetName}-{Guid.NewGuid()}.xlsx";
            string filepath = excelService.PathToExcellFile(fileName);
            bool result = false;
            await Task.Run(() =>
            {
                IsBusy = true;
                IsExportButtonEnabled = false;

                ExcelStructure data = new ExcelStructure();
                
                if (timeSamples.Count > Constants.MaxEntriesForSheet) // freq samples will have same len
                {
                    result = PopulateBigList(data, filepath, shouldExportTimeSamples);
                }
                else
                {
                    result = PopulateSmallList(data, filepath, Constants.ExcelSheetNames[0], shouldExportTimeSamples);
                }

                IsExportButtonEnabled = true;
                IsBusy = false;
            });

            if(result)
            {
                await Launcher.OpenAsync(new OpenFileRequest()
                {
                    File = new ReadOnlyFile(filepath)
                });
            }
            else
            {
                await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                               Resources.Strings.Resource.NoData);
            }
        }

        private bool PopulateList(ExcelStructure data, bool shouldExportTimeDomain)
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
                if(freqSamples.Count() > 0)
                {
                    data.Headers = new List<string>()
                    {
                        Resources.Strings.Resource.Freqz,
                        Resources.Strings.Resource.Sample
                    };
                    for (int i = 0; i < freqSamples.Count(); i++)
                    {
                        List<string> values = new List<string>
                        {
                            freqSamples[i].SampleXToString(), freqSamples[i].SampleYToString()
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
                    return false;
                }
            }
            BusyMessage = Resources.Strings.Resource.ExportingToExcellMessage;
            return true;
        }

        private bool PopulateSmallList(ExcelStructure data, string filepath, string excellSheetName, bool shouldExportTimeDomain)
        {
            excelService.GenerateExcel(filepath);
            if (PopulateList(data, shouldExportTimeDomain))
            {
                excelService.InsertDataIntoSheet(filepath, excellSheetName, data);
                return true;
            }

            return false;
        }

        private bool PopulateBigList(ExcelStructure data, string filepath, bool shouldExportTimeDomain)
        {
            if(PopulateList(data, shouldExportTimeDomain))
            {
                excelService.CreateAndInsertDataToManySheets(filepath, data);
                return true;
            }

            return false;
        }

        private async Task ExportToCSV()
        {
            IsBusy = true;
            IsExportButtonEnabled = false;
            StringBuilder text = new StringBuilder();
            var fileName = $"{Constants.ExcellSheetName}-{Guid.NewGuid()}.csv";
            bool returnError = false;
            await Task.Run(() =>
            {
                if (shouldExportTimeSamples)
                {
                    for(int i = 0; i < timeSamples.Count; i++)
                    {
                        text.AppendLine($"{i}{Constants.Delimeter}{timeSamples[i]}");

                        if (i % 10000 == 0)
                        {
                            BusyMessage = Resources.Strings.Resource.ExcellProcessing + $" {i}/{timeSamples.Count}";
                        }
                    }
                }
                else
                {
                    if(freqSamples.Count() > 0)
                    {
                        for (int i = 0; i < freqSamples.Count(); i++)
                        {
                            text.AppendLine($"{freqSamples[i].SampleXValue}{Constants.Delimeter}{freqSamples[i].SampleYValue}");

                            if (i % 10000 == 0)
                            {
                                BusyMessage = Resources.Strings.Resource.ExcellProcessing + $" {i}/{timeSamples.Count}";
                            }
                        }
                    }
                    else
                    {
                        returnError = true;
                    }
                    
                } 
            });

            if(returnError)
            {
                await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                                Resources.Strings.Resource.NoData);
            }
            else
            {
                excelService.ExportCsvFile(fileName, Constants.ExcellSheetName, text.ToString());
            }

            IsExportButtonEnabled = true;
            IsBusy = false;
        }
        #endregion
        #endregion
    }
}
