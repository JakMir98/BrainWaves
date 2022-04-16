using BrainWaves.Helpers;
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
        private ObservableCollection<double> eegClickSamples = new ObservableCollection<double>();
        private SampleTranformService sampleTransformService;
        private bool areButtonsEnabled = true;
        private string outputText;
        private string entryText;
        private string selectedSettings;
        private List<string> availableSettings;
        private int samplingFreq;
        private float timeToMeasureInMins;
        private float expectedNumberOfSamples;
        private bool isGoToChartsEnabled = false;
        #endregion

        #region ICommands
        public ICommand StartCommand { private set; get; }
        public ICommand SendCommand { private set; get; }
        public ICommand DisconnectCommand { private set; get; }
        public ICommand StartReceivingCommand { private set; get; }
        public ICommand StopReceivingCommand { private set; get; }
        public ICommand GoToChartsCommand { private set; get; }
        public ICommand CalculateCommand { private set; get; }
        public ICommand GoToSettingsCommand { private set; get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Just for testing purpose
        /// </summary>
        public BluetoothDataViewModel()
        {
            //_bluetoothAdapter = CrossBluetoothLE.Current.Adapter; on windows throws exeption if not have ble
            sampleTransformService = new SampleTranformService();
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
            IsGoToChartsEnabled = false;
            //SendCommand = new Command(Send);
            StartCommand = new Command(async () => await StartMeasure());
            DisconnectCommand = new Command(async () => await Disconnect());
            StartReceivingCommand = new Command(async () => await StartReceiving());
            StopReceivingCommand = new Command(StopReceiving);
            GoToChartsCommand = new Command(async () => await GoToChartsPage());
            GoBackCommand = new Command(async () => await Disconnect());
            GoToSettingsCommand = new Command(async () => await GoToSettings());
            CalculateCommand = new Command(Calculate);
        }

        public BluetoothDataViewModel(IDevice connectedDevice)
        {
            _connectedDevice = connectedDevice;
            _bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            sampleTransformService = new SampleTranformService();
            //samples.Capacity = ;
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
            IsGoToChartsEnabled = false;
            StartCommand = new Command(async () => await StartMeasure());
            DisconnectCommand = new Command(async () => await Disconnect());
            StartReceivingCommand = new Command(async () => await StartReceiving());
            StopReceivingCommand = new Command(StopReceiving);
            GoToChartsCommand = new Command(async () => await GoToChartsPage());
            GoBackCommand = new Command(async () => await GoBack());
            GoToSettingsCommand = new Command(async () => await GoToSettings());
            CalculateCommand = new Command(Calculate);
        }
        #endregion

        #region INotify Getters and Setters
        public string OutputText
        {
            get => outputText;
            set => SetProperty(ref outputText, value);
        }

        public bool AreButtonsEnabled
        {
            get => areButtonsEnabled;
            set => SetProperty(ref areButtonsEnabled, value);
        }

        public string EntryText
        {
            get => entryText;
            set => SetProperty(ref entryText, value);
        }

        public ObservableCollection<double> EegClickSamples
        {
            get => eegClickSamples;
            set => SetProperty(ref eegClickSamples, value);
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
        #endregion

        #region Functions
        private async Task StartMeasure()
        {
            AreButtonsEnabled = false;
            EegClickSamples.Clear();
            if (Preferences.Get(Constants.PrefsAutomaticServiceChossing, true))
            {
                await GetCharacteristicWithoutUUID();
            }
            else
            {
                await GetCharacteristic();
            }

            await StartReceiving();

            string message = $"{Constants.StartMeasureStartMessage}{Constants.Delimeter}{samplingFreq}{Constants.Delimeter}{timeToMeasureInMins}";
            expectedNumberOfSamples = samplingFreq * timeToMeasureInMins * 60;
            Send(message);
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
            if (receiveCharacteristic != null)
            {
                receiveCharacteristic.ValueUpdated += ReadValues;

                await receiveCharacteristic.StartUpdatesAsync();
            }
        }

        private void StopReceiving()
        {
            receiveCharacteristic.ValueUpdated -= ReadValues;
        }

        private void ReadValues(object o, CharacteristicUpdatedEventArgs args)
        {
            BusyMessage = "Reading";
            var t = Task.Run(() =>
            {
                var receivedBytes = args.Characteristic.Value;
                var stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                if (string.Equals(stringValue, "End"))
                {
                    AreButtonsEnabled = true;
                    IsGoToChartsEnabled = true;
                }
                else
                {
                    EegClickSamples.Add(sampleTransformService.ConvertToVoltage(stringValue));
                    OutputText = $"Received: {EegClickSamples.Count}/{expectedNumberOfSamples}";
                }
            });
            t.Wait();
        }

        private async Task GoToChartsPage()
        {
            IsBusy = true;
            BusyMessage = Resources.Strings.Resource.OpenPageText;
            await OpenPage(new ChartsPage(new List<double>(EegClickSamples)));
            IsBusy = false;
        }

        private async Task GoToSettings()
        {
            await OpenPage(new SettingsPage());
        }

        private async void Calculate()
        {
            await Task.Run(() =>
            {
                IsGoToChartsEnabled = false;
                IsBusy = true;
                BusyMessage = "Generating long sinwave";
                int samplingFreq = 500;
                int length = 1048576; //2^20
                double[] sinWave = HelperFunctions.GenerateSinWave(samplingFreq, length, 1, 50);
                EegClickSamples = new ObservableCollection<double>(sinWave);
                IsBusy = false;
                IsGoToChartsEnabled = true;
            });
            
            /*
            try
            {
                IsBusy = true;
                BusyMessage = Resources.Strings.Resource.SendingMessage;
                if (sendCharacteristic != null)
                {
                    int hz = Preferences.Get(Constants.PrefsSavedTimeToReadMindInMinutes, Constants.MinTimeToReadInMinutes);
                    int time = Preferences.Get(Constants.PrefsSavedTimeToReadMindInMinutes, Constants.MinTimeToReadInMinutes);
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.UTF8.GetBytes($"start;{hz};{time}"));
                }
            }
            catch (Exception ex)
            {
                OutputText += $"Error sending comand to UART. {ex.Message}" + Environment.NewLine;
            }
            finally
            {
                IsBusy = false;
            }
            */
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
        #endregion
    }
}
