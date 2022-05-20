using BrainWaves.Helpers;
using BrainWaves.Models;
using BrainWaves.Services;
using BrainWaves.Views;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BrainWaves.ViewModels
{
    public class BluetoothDataViewModel : BaseViewModel
    {
        #region Variables
        private readonly IDevice _connectedDevice;
        private readonly IAdapter _bluetoothAdapter;
        private ICharacteristic sendCharacteristic;
        private ICharacteristic receiveCharacteristic;
        private ObservableCollection<double> dataFromBleDevice = new ObservableCollection<double>();
        private List<List<double>> dataToWavesPage = new List<List<double>>();
        private SampleTranformService sampleTransformService;
        private int samplingFreq;
        private float timeToMeasureInMins;
        private float expectedNumberOfSamples;
        private int dataPartCounter;
        #region ViewVariables
        private string outputText;
        private string selectedSettings;
        private List<string> availableSettings;
        private bool isReadButtonEnabled = true;
        private bool isGoToChartsEnabled = false;
        private bool isGoToWavesChartsEnabled = false;
        private bool isGenerateSinwaveVisible = false;
        private GenerateSinwaveViewModel sinwaveModel;
        private GameViewModel gameModel;
        private double progress;
        private bool progressBarIsVisible = false;
        private string selectedMeasurement;
        private bool isTimeFreqMesVisible;
        private bool isWavesVisible;
        private bool isTestSigVisible;
        #endregion
        #endregion

        #region ICommands
        public ICommand StartCommand { private set; get; }
        public ICommand StartOnehourMeasurementCommand { private set; get; }
        public ICommand SendCommand { private set; get; }
        public ICommand GoToChartsCommand { private set; get; }
        public ICommand GenerateCommand { private set; get; }
        public ICommand GoToSettingsCommand { private set; get; }
        public ICommand SendTestSignalCommand { private set; get; }
        public ICommand GoToWaveChartsPageCommand { private set; get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Just for testing purpose
        /// </summary>
        public BluetoothDataViewModel()
        {
            //_bluetoothAdapter = CrossBluetoothLE.Current.Adapter; on windows throws exeption if not have ble
            InitVariables();
            InitCommands();
        }

        public BluetoothDataViewModel(IDevice connectedDevice)
        {
            _connectedDevice = connectedDevice;
            _bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            //samples.Capacity = ;
            InitVariables();
            InitCommands();
        }
        #endregion

        #region INotify Getters and Setters
        public string OutputText
        {
            get => outputText;
            set => SetProperty(ref outputText, value);
        }

        public bool IsReadButtonEnabled
        {
            get => isReadButtonEnabled;
            set => SetProperty(ref isReadButtonEnabled, value);
        }

        public ObservableCollection<double> DataFromBleDevice
        {
            get => dataFromBleDevice;
            set => SetProperty(ref dataFromBleDevice, value);
        }
        
        public string SelectedSettings
        {
            get => selectedSettings;
            set
            {
                SetProperty(ref selectedSettings, value);
                SelectedSettingsTimeFreqChangedHandle(value);
            }
        }

        public List<string> AvailableSettings
        {
            get => availableSettings;
            set => SetProperty(ref availableSettings, value);
        }

        public bool IsGoToChartsEnabled
        {
            get => isGoToChartsEnabled;
            set => SetProperty(ref isGoToChartsEnabled, value);
        }

        public bool IsGenerateSinwaveVisible
        {
            get => isGenerateSinwaveVisible;
            set
            {
                SetProperty(ref isGenerateSinwaveVisible, value);
                IsReadButtonEnabled = !value;
            }
        }

        public GameViewModel GameViewModel
        {
            get => gameModel;
            set => SetProperty(ref gameModel, value);
        }
        public GenerateSinwaveViewModel SinwaveViewModel
        {
            get => sinwaveModel;
            set => SetProperty(ref sinwaveModel, value);
        }

        public double Progress
        {
            get => progress;
            set => SetProperty(ref progress, value);
        }

        public bool ProgressBarIsVisible
        {
            get => progressBarIsVisible;
            set => SetProperty(ref progressBarIsVisible, value);
        }

        public string SelectedMeasurement
        {
            get => selectedMeasurement;
            set
            {
                SetProperty(ref selectedMeasurement, value);
                HandleSelectedMeasurement(value);
            }
        }
        public bool IsTimeFreqMesVisible
        {
            get => isTimeFreqMesVisible;
            set => SetProperty(ref isTimeFreqMesVisible, value);
        }

        public bool IsWavesVisible
        {
            get => isWavesVisible;
            set => SetProperty(ref isWavesVisible, value);
        }

        public bool IsTestSigVisible
        {
            get => isTestSigVisible;
            set => SetProperty(ref isTestSigVisible, value);
        }

        public bool IsGoToWavesChartsEnabled
        {
            get => isGoToWavesChartsEnabled;
            set => SetProperty(ref isGoToWavesChartsEnabled, value);
        }
        #endregion

        #region Functions
        private void InitVariables()
        {
            sampleTransformService = new SampleTranformService();
            gameModel = new GameViewModel();
            sinwaveModel = new GenerateSinwaveViewModel();

            Title = Resources.Strings.Resource.BLEData;
            AvailableSettings = new List<string>
            {
                Resources.Strings.Resource.StartSettingTest,
                Resources.Strings.Resource.StartSettingUserDefined,
                Resources.Strings.Resource.StartSettingMinimal,
                Resources.Strings.Resource.StartSettingBasic,
                Resources.Strings.Resource.StartSettingShortHighSampleRate,
                Resources.Strings.Resource.StartSettingLongLowSampleRate,
                Resources.Strings.Resource.StartSettingAdvanced,
                Resources.Strings.Resource.StartSettingFull
            };
            SelectedSettings = Resources.Strings.Resource.StartSettingUserDefined;   
            SelectedMeasurement = Resources.Strings.Resource.TimeFreqMeasurement;
        }

        private void InitCommands()
        {
            StartCommand = new Command(async () => await StartTimeFreqMeasure());
            GoToChartsCommand = new Command(async () => await GoToChartsPage());
            GoBackCommand = new Command(async () => await Disconnect());
            GoToSettingsCommand = new Command(async () => await GoToSettings());
            GenerateCommand = new Command(Generate);
            SendTestSignalCommand = new Command(async () => await SendTestSignal());
            GoToWaveChartsPageCommand = new Command(async () => await GoToWavesChartPage());
            StartOnehourMeasurementCommand = new Command(async () => await StartOneHourMeasure());
        }

        #region Time Freq Measurements
        private async Task StartTimeFreqMeasure()
        {
            IsReadButtonEnabled = false;
            IsGoToChartsEnabled = false;

            DataFromBleDevice.Clear();
            await StartReceiving(MeasurementType.TIME_FREQUENCY_MEASUREMENT);
            string message = $"{Constants.StartMeasureStartMessage}{Constants.Delimeter}{samplingFreq}{Constants.Delimeter}{timeToMeasureInMins}";
            expectedNumberOfSamples = samplingFreq * timeToMeasureInMins * 60;
            Send(message);

            ProgressBarIsVisible = true;
            gameModel.SetupGame();
        }

        private void ReadTimeFreqValues(object o, CharacteristicUpdatedEventArgs args)
        {
            BusyMessage = Resources.Strings.Resource.ReadingText;
            Task.Run(() =>
            {
                var receivedBytes = args.Characteristic.Value;
                var stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                if (string.Equals(stringValue, Constants.EndMeasureEndMessage))
                {
                    StopReceiving(MeasurementType.TIME_FREQUENCY_MEASUREMENT);
                    IsReadButtonEnabled = true;
                    IsGoToChartsEnabled = true;
                    gameModel.StopwatchGame.Reset();
                    ProgressBarIsVisible = false;
                }
                else
                {
                    DataFromBleDevice.Add(sampleTransformService.ConvertToVoltage(stringValue));
                    OutputText = $"{Resources.Strings.Resource.ReceivedDataText}: {DataFromBleDevice.Count}/{expectedNumberOfSamples}";
                    // todo check possible loss of samples 
                    // these actions slows down receiving samples
                    UpdateProgressBar(expectedNumberOfSamples);
                    gameModel.UpdateUiGame(timeToMeasureInMins);
                }
            });
        }

        private void SelectedSettingsTimeFreqChangedHandle(string setting)
        {
            if (setting == Resources.Strings.Resource.StartSettingTest)
            {
                samplingFreq = 80;
                timeToMeasureInMins = 0.25f;
            }
            else if (setting == Resources.Strings.Resource.StartSettingUserDefined)
            {
                samplingFreq = Preferences.Get(Constants.PrefsSavedSamplingFrequency, Constants.MinSamplingFrequency);
                timeToMeasureInMins = Preferences.Get(Constants.PrefsSavedTimeToReadMindInMinutes, Constants.MinTimeToReadInMinutes);
            }
            else if (setting == Resources.Strings.Resource.StartSettingMinimal)
            {
                samplingFreq = 80;
                timeToMeasureInMins = 1;
            }
            else if (setting == Resources.Strings.Resource.StartSettingBasic)
            {
                samplingFreq = 160;
                timeToMeasureInMins = 15;
            }
            else if (setting == Resources.Strings.Resource.StartSettingShortHighSampleRate)
            {
                samplingFreq = 500;
                timeToMeasureInMins = 5;
            }
            else if (setting == Resources.Strings.Resource.StartSettingLongLowSampleRate)
            {
                samplingFreq = 80;
                timeToMeasureInMins = 30;
            }
            else if (setting == Resources.Strings.Resource.StartSettingAdvanced)
            {
                samplingFreq = 500;
                timeToMeasureInMins = 15;
            }
            else if (setting == Resources.Strings.Resource.StartSettingFull)
            {
                samplingFreq = 500;
                timeToMeasureInMins = 60;
            }
        }
        #endregion

        #region Waves Measurements
        private async Task StartOneHourMeasure()
        {
            IsReadButtonEnabled = false;
            IsGoToWavesChartsEnabled = false;

            DataFromBleDevice.Clear();
            dataPartCounter = 0;
            await StartReceiving(MeasurementType.WAVES_MEASUREMENT);
            SendStartTenMinuteMeasurementMessage();

            ProgressBarIsVisible = true;
            gameModel.SetupGame();
        }

        private void SendStartTenMinuteMeasurementMessage()
        {
            int fs = 200;
            int measureInMin = 10;
            string message = $"{Constants.StartWavesMeasureStartMessage}{Constants.Delimeter}{fs}{Constants.Delimeter}{measureInMin}";
            expectedNumberOfSamples = fs * measureInMin * 60; // freq * timeInMinutes * scalingToMinutes
            Send(message);
        }

        private void ReadOneHourMeasurement(object o, CharacteristicUpdatedEventArgs args)
        {
            BusyMessage = Resources.Strings.Resource.ReadingText;
            Task.Run(() =>
            {
                var receivedBytes = args.Characteristic.Value;
                var stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                if (string.Equals(stringValue, Constants.EndMeasureEndMessage))
                {
                    dataPartCounter++;
                    dataToWavesPage.Add(new List<double>(DataFromBleDevice));
                    DataFromBleDevice.Clear();
                    if (dataPartCounter >= Constants.DefaultNumOfMeasurementsForWaves)
                    {
                        StopReceiving(MeasurementType.WAVES_MEASUREMENT);
                        IsReadButtonEnabled = true;
                        IsGoToWavesChartsEnabled = true;
                        ProgressBarIsVisible = false;
                    }
                    else
                    {
                        SendStartTenMinuteMeasurementMessage();
                        gameModel.LabelText = Resources.Strings.Resource.TimeForRelaxText;
                    }
                    gameModel.StopwatchGame.Reset();
                }
                else
                {
                    if (double.TryParse(stringValue, out var value))
                    {
                        DataFromBleDevice.Add(value); // raw value
                    }
                    UpdateProgressBar(expectedNumberOfSamples);
                    gameModel.UpdateUiGame(timeToMeasureInMins);
                }
            });
        }

        private async Task GoToWavesChartPage()
        {
            IsBusy = true;
            /* TESTING */
            dataToWavesPage = TestSamplesGenerator.GenerateWavesSamples(128); // todo delete 
            //dataToWavesPage = TestSamplesGenerator.GenerateRandomWavesSamples(128);
            /*TESTING */

            List<BrainWaveSample> brainWavesSamples = new List<BrainWaveSample>();
            foreach (var timeSamples in dataToWavesPage)
            {
                List<Sample> freqSamples = new List<Sample>();
                await Task.Run(() =>
                {
                    BusyMessage = Resources.Strings.Resource.CalculateFFT;
                    freqSamples = HelperFunctions.GenerateFreqSamples(timeSamples.ToArray(), 200);
                });

                brainWavesSamples.Add(HelperFunctions.GenerateBrainWavesSampleFromFFTWavesSamples(freqSamples));
            }
            IsBusy = false;
            await OpenPage(new WavesPage(brainWavesSamples));
        }
        #endregion

        #region Test signal
        private void ReadTestValues(object o, CharacteristicUpdatedEventArgs args)
        {
            BusyMessage = Resources.Strings.Resource.ReadingText;
            Task.Run(() =>
            {
                var receivedBytes = args.Characteristic.Value;
                var stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                if (string.Equals(stringValue, Constants.EndMeasureEndMessage))
                {
                    StopReceiving(MeasurementType.TEST_MEASUREMENT);
                    IsReadButtonEnabled = true;
                    ProgressBarIsVisible = false;
                }
                else
                {
                    if (double.TryParse(stringValue, out var value))
                    {
                        DataFromBleDevice.Add(value);
                    }
                    UpdateProgressBar(expectedNumberOfSamples);// todo check len of look up table
                }
            });
        }

        private async Task SendTestSignal()
        {
            await StartReceiving(MeasurementType.TEST_MEASUREMENT);
            string message = $"{Constants.TestSingalMessage}";
            Send(message);
        }

        private async void Generate()
        {
            await Task.Run(() =>
            {
                IsReadButtonEnabled = false;
                IsGoToChartsEnabled = false;
                IsBusy = true;
                BusyMessage = Resources.Strings.Resource.GenerateSinwave;
                double[] sinWave = TestSamplesGenerator.GenerateSinWave(
                        sinwaveModel.SinwaveSamplingFreq, sinwaveModel.SinwaveLength, sinwaveModel.SinwaveAmplitude, sinwaveModel.SinwaveFrequency);

                HelperFunctions.PerformZeroPaddingIfNeeded(ref sinWave);

                DataFromBleDevice = new ObservableCollection<double>(sinWave);
                IsBusy = false;
                IsGoToChartsEnabled = true;
                IsReadButtonEnabled = true;
            });
        }
        #endregion

        #region Bluetooth
        private async Task GetCharacteristic()
        {
            try
            {
                IsBusy = true;
                BusyMessage = Resources.Strings.Resource.GettingCharacteristicMessage;
                var service = await _connectedDevice.GetServiceAsync(Guid.Parse
                    (Preferences.Get(Constants.PrefsSavedServiceUUID, Constants.UartGattServiceId.ToString())));

                if (service != null)
                {
                    sendCharacteristic = await service.GetCharacteristicAsync(Guid.Parse
                        (Preferences.Get(Constants.PrefsSavedSendCharacteristicUUID, Constants.GattCharacteristicReceiveId.ToString())));
                    receiveCharacteristic = await service.GetCharacteristicAsync(Guid.Parse
                        (Preferences.Get(Constants.PrefsSavedReceiveCharacteristicUUID, Constants.GattCharacteristicSendId.ToString())));
                }
                else
                {
                    await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                        Resources.Strings.Resource.GattServiceNotFoundError);
                }
            }
            catch (Exception ex)
            {
                await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                        Resources.Strings.Resource.GattInitError + $" {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GetCharacteristicWithoutUUID()
        {
            try
            {
                IsBusy = true;
                BusyMessage = Resources.Strings.Resource.GettingCharacteristicMessage;
                var services = await _connectedDevice.GetServicesAsync();
                foreach (var service in services)
                {
                    if (service != null)
                    {
                        var characteristics = await service.GetCharacteristicsAsync();
                        foreach (var characteristic in characteristics)
                        {
                            if (characteristic.CanWrite)
                            {
                                sendCharacteristic = characteristic;
                            }
                            else if (characteristic.CanUpdate)
                            {
                                receiveCharacteristic = characteristic;
                            }
                        }
                    }
                    else
                    {
                        await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                            Resources.Strings.Resource.GattServiceNotFoundError);
                    }
                }
            }
            catch (Exception ex)
            {
                await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                        Resources.Strings.Resource.GattInitError + $" {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void Send(string message)
        {
            try
            {
                IsBusy = true;
                BusyMessage = Resources.Strings.Resource.SendingMessage;
                if (sendCharacteristic != null)
                {
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.UTF8.GetBytes($"{message}\r\n"));
                }
            }
            catch (Exception ex)
            {
                await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                    Resources.Strings.Resource.BleCommandSendingError + $" {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Disconnect()
        {
            try
            {
                IsBusy = true;
                BusyMessage = Resources.Strings.Resource.DisconnectingMessage;
                await _bluetoothAdapter.DisconnectDeviceAsync(_connectedDevice);
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch
            {
                await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                    Resources.Strings.Resource.BleDisconnectError);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task StartReceiving(MeasurementType type)
        {
            try
            {
                if (receiveCharacteristic != null)
                {
                    switch (type)
                    {
                        case MeasurementType.TIME_FREQUENCY_MEASUREMENT:
                            receiveCharacteristic.ValueUpdated += ReadTimeFreqValues;
                            break;
                        case MeasurementType.TEST_MEASUREMENT:
                            receiveCharacteristic.ValueUpdated += ReadTestValues;
                            break;
                        case MeasurementType.WAVES_MEASUREMENT:
                            receiveCharacteristic.ValueUpdated += ReadOneHourMeasurement;
                            break;
                    }
                    await receiveCharacteristic.StartUpdatesAsync();
                }
            }
            catch (Exception ex)
            {
                await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                    Resources.Strings.Resource.BleCommandSendingError + $" {ex.Message}");
            }
        }

        private void StopReceiving(MeasurementType type)
        {
            switch (type)
            {
                case MeasurementType.TIME_FREQUENCY_MEASUREMENT:
                    receiveCharacteristic.ValueUpdated -= ReadTimeFreqValues;
                    break;
                case MeasurementType.TEST_MEASUREMENT:
                    receiveCharacteristic.ValueUpdated -= ReadTestValues;
                    break;
                case MeasurementType.WAVES_MEASUREMENT:
                    receiveCharacteristic.ValueUpdated -= ReadOneHourMeasurement;
                    break;
            }
        }

        public async Task SetupCharacteristicAsync()
        {
            if (_connectedDevice != null)
            {
                if (Preferences.Get(Constants.PrefsAutomaticServiceChossing, true))
                {
                    await GetCharacteristicWithoutUUID();
                }
                else
                {
                    await GetCharacteristic();
                }
            }
        }
        #endregion

        private void UpdateProgressBar(float maxReceivedNumOfSamples)
        {
            OutputText = $"{Resources.Strings.Resource.ReceivedDataText}: {DataFromBleDevice.Count}/{maxReceivedNumOfSamples}";

            Progress = DataFromBleDevice.Count / maxReceivedNumOfSamples;
        }
       
        private async Task GoToChartsPage()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.OpenPageText;
            if(IsGenerateSinwaveVisible)
            {
                await OpenPage(new ChartsPage(new List<double>(DataFromBleDevice), sinwaveModel.SinwaveSamplingFreq));
            }
            else
            {
                if(DataFromBleDevice.Count > 0)
                {
                    await OpenPage(new ChartsPage(new List<double>(DataFromBleDevice), samplingFreq));
                }
                else
                {
                    await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle,
                                Resources.Strings.Resource.NoData);
                }
            }

            IsBusy = false;
        }

        public void HandleSelectedMeasurement(string value)
        {
            if(value == Resources.Strings.Resource.TimeFreqMeasurement)
            {
                IsTimeFreqMesVisible = true;
                IsWavesVisible = false;
                IsTestSigVisible = false;
                gameModel.IsGameVisible = true;
            }
            else if(value == Resources.Strings.Resource.WavesMeasurements)
            {
                IsTimeFreqMesVisible = false;
                IsWavesVisible = true;
                IsTestSigVisible = false;
                gameModel.IsGameVisible = true;
            }
            else if(value == Resources.Strings.Resource.TestMeasurements)
            {
                IsTimeFreqMesVisible = false;
                IsWavesVisible = false;
                IsTestSigVisible = true;
                gameModel.IsGameVisible = false;
            }
        }
        #endregion
    }
}
