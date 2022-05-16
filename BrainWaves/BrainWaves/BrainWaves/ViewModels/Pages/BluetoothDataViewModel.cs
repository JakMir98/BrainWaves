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
        private SampleTranformService sampleTransformService;
        private string outputText;
        private string selectedSettings;
        private List<string> availableSettings;
        private int samplingFreq;
        private float timeToMeasureInMins;
        private float expectedNumberOfSamples;
        private bool isReadButtonEnabled = true;
        private bool isGoToChartsEnabled = false;
        private bool isGenerateSinwaveVisible = false;
        private GenerateSinwaveViewModel sinwaveModel; 
        private GameViewModel gameModel;
        private double progress;
        private bool progressBarIsVisible = false;
        private bool isTestMessage;
        private string selectedMeasurement;
        private bool isTimeFreqMesVisible;
        private bool isWavesVisible;
        private bool isTestSigVisible;
        private bool isGameVisible;
        private int dataPartCounter = 0;
        List<List<BrainWaveSample>> brainWavesSamples = new List<List<BrainWaveSample>>();
        #endregion

        #region ICommands
        public ICommand StartCommand { private set; get; }
        public ICommand StartOnehourMeasurementCommand { private set; get; }
        public ICommand SendCommand { private set; get; }
        public ICommand GoToChartsCommand { private set; get; }
        public ICommand GenerateCommand { private set; get; }
        public ICommand GoToSettingsCommand { private set; get; }
        public ICommand CheckCommand { private set; get; }
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
                SelectedSettingsChangedHandle(value);
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

        public GameViewModel Game
        {
            get => gameModel;
            set => SetProperty(ref gameModel, value);
        }
        public GenerateSinwaveViewModel SinwaveModel
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

        public bool IsGameVisible
        {
            get => isGameVisible;
            set => SetProperty(ref isGameVisible, value);
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
            StartCommand = new Command(async () => await StartMeasure());
            GoToChartsCommand = new Command(async () => await GoToChartsPage());
            GoBackCommand = new Command(async () => await Disconnect());
            GoToSettingsCommand = new Command(async () => await GoToSettings());
            GenerateCommand = new Command(Generate);
            CheckCommand = new Command(CheckExercise);
            SendTestSignalCommand = new Command(async () => await SendTestSignal());
            GoToWaveChartsPageCommand = new Command(async () => await GoToWavesChartPage());
            StartOnehourMeasurementCommand = new Command(async () => await StartOneHourMeasure());
        }

        private async Task StartMeasure()
        {
            isTestMessage = false;
            IsReadButtonEnabled = false;
            IsGoToChartsEnabled = false;

            DataFromBleDevice.Clear();
            await StartReceiving();
            string message = $"{Constants.StartMeasureStartMessage}{Constants.Delimeter}{samplingFreq}{Constants.Delimeter}{timeToMeasureInMins}";
            expectedNumberOfSamples = samplingFreq * timeToMeasureInMins * 60;
            Send(message);

            ProgressBarIsVisible = true;
            gameModel.StopwatchGame.Start();
            gameModel.IsBrainRelaxViewVisible = true;
            gameModel.IsBrainActivityViewVisible = false;
            gameModel.LabelText = Resources.Strings.Resource.TimeForRelaxText;
        }

        private async Task StartOneHourMeasure()
        {

            /*
            isTestMessage = false;
            IsReadButtonEnabled = false;
            IsGoToChartsEnabled = false;

            DataFromBleDevice.Clear();
            dataPartCounter = 0;
            await StartReceiving();
            string message = $"{Constants.StartMeasureStartMessage}{Constants.Delimeter}{samplingFreq}{Constants.Delimeter}{timeToMeasureInMins}";
            expectedNumberOfSamples = 200 * 10 * 60; // freq * timeInMinutes*scalingToMinutes
            Send(message);

            ProgressBarIsVisible = true;
            gameModel.StopwatchGame.Start();
            gameModel.IsBrainRelaxViewVisible = true;
            gameModel.IsBrainActivityViewVisible = false;
            gameModel.LabelText = Resources.Strings.Resource.TimeForRelaxText;
            */
        }

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
                        Resources.Strings.Resource.GattInitError +$" {ex.Message}" );
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
                StopReceiving();
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

        private async Task StartReceiving()
        {
            try
            {
                if (receiveCharacteristic != null)
                {
                    if(isTestMessage)
                    {
                        receiveCharacteristic.ValueUpdated += ReadTestValues;
                    }
                    else if(!isTestMessage && isTimeFreqMesVisible)
                    {
                        receiveCharacteristic.ValueUpdated += ReadEEGValues;
                    }
                    else
                    {
                        receiveCharacteristic.ValueUpdated += ReadOneHourMeasurement;
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

        private void StopReceiving()
        {
            if (isTestMessage)
            {
                receiveCharacteristic.ValueUpdated -= ReadTestValues;
            }
            else if (!isTestMessage && isTimeFreqMesVisible)
            {
                receiveCharacteristic.ValueUpdated -= ReadEEGValues;
            }
            else
            {
                receiveCharacteristic.ValueUpdated -= ReadOneHourMeasurement;
            }
        }

        private void ReadEEGValues(object o, CharacteristicUpdatedEventArgs args)
        {
            BusyMessage = Resources.Strings.Resource.ReadingText;
            Task.Run(() =>
            {
                var receivedBytes = args.Characteristic.Value;
                var stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                if (string.Equals(stringValue, Constants.EndMeasureEndMessage))
                {
                    StopReceiving();
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
                    Progress = DataFromBleDevice.Count / expectedNumberOfSamples;
                    if (gameModel.IsBrainActivityVisible)
                    {
                        if (gameModel.StopwatchGame.Elapsed.TotalMinutes > timeToMeasureInMins / 2)
                        {
                            gameModel.IsBrainActivityViewVisible = true;
                            gameModel.LabelText = Resources.Strings.Resource.TimeToFocusText;
                        }

                        if (!gameModel.IsBrainActivityViewVisible && DataFromBleDevice.Count % 100 == 0)
                        {
                            gameModel.LabelText = $"{Resources.Strings.Resource.TimeForRelaxText}";
                        }
                    }
                }
            });
        }

        private void ReadTestValues(object o, CharacteristicUpdatedEventArgs args)
        {
            BusyMessage = Resources.Strings.Resource.ReadingText;
            Task.Run(() =>
            {
                var receivedBytes = args.Characteristic.Value;
                var stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                if (string.Equals(stringValue, Constants.EndMeasureEndMessage))
                {
                    StopReceiving();
                    IsReadButtonEnabled = true;
                    ProgressBarIsVisible = false;
                }
                else
                {
                    if(double.TryParse(stringValue, out var value))
                    {
                        DataFromBleDevice.Add(value);
                    }
                    OutputText = $"{Resources.Strings.Resource.ReceivedDataText}: {DataFromBleDevice.Count}/{expectedNumberOfSamples}"; // todo check len of look up table
                    // todo check possible loss of samples 
                    // these actions slows down receiving samples
                    Progress = DataFromBleDevice.Count / expectedNumberOfSamples;// todo check len of look up table

                }
            });
        }

        private void ReadOneHourMeasurement(object o, CharacteristicUpdatedEventArgs args)
        {//todo upgrade
            BusyMessage = Resources.Strings.Resource.ReadingText;
            Task.Run(() =>
            {
                var receivedBytes = args.Characteristic.Value;
                var stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                if (string.Equals(stringValue, Constants.EndMeasureEndMessage))
                {
                    dataPartCounter++;
                    if(dataPartCounter >= Constants.DefaultNumOfMeasurementsForWaves)
                    {
                        StopReceiving();
                        IsReadButtonEnabled = true;
                        IsGoToChartsEnabled = true;
                        gameModel.StopwatchGame.Reset();
                        ProgressBarIsVisible = false;
                    }
                    else
                    {
                        //todo send another start signal
                    }
                }
                else
                {
                    if (double.TryParse(stringValue, out var value))
                    {
                        DataFromBleDevice.Add(value); // raw value
                    }

                    OutputText = $"{Resources.Strings.Resource.ReceivedDataText}: {DataFromBleDevice.Count}/{expectedNumberOfSamples}";

                    Progress = DataFromBleDevice.Count / expectedNumberOfSamples;
                    if (gameModel.IsBrainActivityVisible)
                    {
                        if (gameModel.StopwatchGame.Elapsed.TotalMinutes > timeToMeasureInMins / 2)
                        {
                            gameModel.IsBrainActivityViewVisible = true;
                            gameModel.LabelText = Resources.Strings.Resource.TimeToFocusText;
                        }

                        if (!gameModel.IsBrainActivityViewVisible && DataFromBleDevice.Count % 100 == 0)
                        {
                            gameModel.LabelText = $"{Resources.Strings.Resource.TimeForRelaxText}";
                        }
                    }
                }
            });
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

        private async void Generate()
        {
            await Task.Run(() =>
            {
                IsReadButtonEnabled = false;
                IsGoToChartsEnabled = false;
                IsBusy = true;
                BusyMessage = Resources.Strings.Resource.GenerateSinwave;
                double[] sinWave;
                
                if (FftSharp.Pad.IsPowerOfTwo(sinwaveModel.SinwaveLength))
                {
                    sinWave = TestSamplesGenerator.GenerateSinWave(
                        sinwaveModel.SinwaveSamplingFreq, sinwaveModel.SinwaveLength, sinwaveModel.SinwaveAmplitude, sinwaveModel.SinwaveFrequency);
                }
                else
                {// zero padding so array is power of 2
                    sinWave = FftSharp.Pad.ZeroPad(TestSamplesGenerator.GenerateSinWave(
                        sinwaveModel.SinwaveSamplingFreq, sinwaveModel.SinwaveLength, sinwaveModel.SinwaveAmplitude, sinwaveModel.SinwaveFrequency));
                }

                DataFromBleDevice = new ObservableCollection<double>(sinWave);
                IsBusy = false;
                IsGoToChartsEnabled = true;
                IsReadButtonEnabled = true;
            });
        }

        private void SelectedSettingsChangedHandle(string setting)
        {
            if(setting == Resources.Strings.Resource.StartSettingTest)
            {
                samplingFreq = 80;
                timeToMeasureInMins = 0.25f;
            }
            else if(setting == Resources.Strings.Resource.StartSettingUserDefined)
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

        private void CheckExercise()
        {
            if(int.TryParse(gameModel.AnswerEntryText, out var number))
            {
                if (gameModel.gameService.CurrentAnswer == number)
                {
                    gameModel.AnswerColor = Color.Green;
                    gameModel.TotalCorrectAnswersCounter++;
                }
                else
                {
                    gameModel.AnswerColor = Color.Red;
                }
            }
            else
            {
                gameModel.AnswerColor = Color.Red;
            }
            
            gameModel.AnswerEntryText = string.Empty;
            (gameModel.gameService.CurrentAnswer, gameModel.QuestionLabelText) = gameModel.gameService.GenerateExercise();
            gameModel.CorrectAnswersText = $"{Resources.Strings.Resource.CorrectAnswersText}{gameModel.TotalCorrectAnswersCounter}/{gameModel.TotalExerciseCounter}";
            gameModel.TotalExerciseCounter++;
            gameModel.ExerciseCounter++;
            if(gameModel.ExerciseCounter > 4 * Constants.DefaultNumOfExercisesToChangeLevel)
            {
                gameModel.ExerciseCounter = 0;
            }
            else if (gameModel.ExerciseCounter > 3 * Constants.DefaultNumOfExercisesToChangeLevel)
            {
                gameModel.gameService.Level = DifficultyLevel.ULTRA;
            }
            else if (gameModel.ExerciseCounter > 2 * Constants.DefaultNumOfExercisesToChangeLevel)
            {
                gameModel.gameService.Level = DifficultyLevel.HARD;
            }
            else if (gameModel.ExerciseCounter > Constants.DefaultNumOfExercisesToChangeLevel)
            {
                gameModel.gameService.Level = DifficultyLevel.MEDIUM;
            }
            else if (gameModel.ExerciseCounter < Constants.DefaultNumOfExercisesToChangeLevel)
            {
                gameModel.gameService.Level = DifficultyLevel.EASY;
            }
        }

        private async Task SendTestSignal()
        {
            isTestMessage = true;
            await StartReceiving();
            string message = $"{Constants.TestSingalMessage}";
            Send(message);
        }

        public async Task SetupCharacteristicAsync()
        {
            if(_connectedDevice != null)
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

        private async Task GoToWavesChartPage()
        {
            List<List<BrainWaveSample>> brainWavesSamples = new List<List<BrainWaveSample>>();
            /*Generate random values*/
            for (int i = 0; i < Constants.DefaultNumOfMeasurementsForWaves; i++)
            {
                List<BrainWaveSample> sample = TestSamplesGenerator.GenerateBrainWaveSamples(500);
                brainWavesSamples.Add(sample);
            }
            
            await OpenPage(new WavesPage(brainWavesSamples));
        }

        private void HandleSelectedMeasurement(string value)
        {
            if(value == Resources.Strings.Resource.TimeFreqMeasurement)
            {
                IsTimeFreqMesVisible = true;
                IsWavesVisible = false;
                IsTestSigVisible = false;
                IsGameVisible = true;
            }
            else if(value == Resources.Strings.Resource.WavesMeasurements)
            {
                IsTimeFreqMesVisible = false;
                IsWavesVisible = true;
                IsTestSigVisible = false;
                IsGameVisible = true;
            }
            else if(value == Resources.Strings.Resource.TestMeasurements)
            {
                IsTimeFreqMesVisible = false;
                IsWavesVisible = false;
                IsTestSigVisible = true;
                IsGameVisible = false;
            }
        }

        #endregion
    }
}
