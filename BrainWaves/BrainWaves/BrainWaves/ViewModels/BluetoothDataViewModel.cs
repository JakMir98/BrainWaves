using BrainWaves.Helpers;
using BrainWaves.Popups;
using BrainWaves.Views;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BrainWaves.ViewModels
{
    public class BluetoothDataViewModel : BaseViewModel
    {
        private readonly IDevice _connectedDevice;
        private readonly IAdapter _bluetoothAdapter;
        private ICharacteristic sendCharacteristic;
        private ICharacteristic receiveCharacteristic;
        List<byte> buffer = new List<byte>();
        private List<string> samples = new List<string>();

        private bool areButtonsEnabled = false;
        private string outputText;
        private string entryText;

        public ICommand InitalizeConnectionCommand { private set; get; }
        public ICommand SendCommand { private set; get; }
        public ICommand DisconnectCommand { private set; get; }
        public ICommand StartReceivingCommand { private set; get; }
        public ICommand StopReceivingCommand { private set; get; }
        public ICommand GoToChartsCommand { private set; get; }

        public BluetoothDataViewModel(IDevice connectedDevice)
        {
            _connectedDevice = connectedDevice;
            _bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            //samples.Capacity = ;
            Title = Resources.Strings.Resource.BLEData;
            if(Preferences.Get(Constants.PrefsAutomaticServiceChossing, true))
            {
                InitalizeConnectionCommand = new Command(async () => await GetCharacteristicWithoutUUID());
            }
            else
            {
                InitalizeConnectionCommand = new Command(async () => await GetCharacteristic());
            }
            
            SendCommand = new Command(Send);
            DisconnectCommand = new Command(async () => await Disconnect());
            StartReceivingCommand = new Command(StartReceiving);
            StopReceivingCommand = new Command(StopReceiving);
            GoToChartsCommand = new Command(async () => await GoToChartsPage());
            GoBackCommand = new Command(async () => await GoBack());
        }

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

        private async Task GetCharacteristic()
        {
            try
            {
                var service = await _connectedDevice.GetServiceAsync(Guid.Parse
                    (Preferences.Get(Constants.PrefsSavedServiceUUID, Constants.UartGattServiceId.ToString())));

                if (service != null)
                {
                    sendCharacteristic = await service.GetCharacteristicAsync(Guid.Parse
                        (Preferences.Get(Constants.PrefsSavedSendCharacteristicUUID, Constants.GattCharacteristicReceiveId.ToString())));
                    receiveCharacteristic = await service.GetCharacteristicAsync(Guid.Parse
                        (Preferences.Get(Constants.PrefsSavedReceiveCharacteristicUUID, Constants.GattCharacteristicSendId.ToString())));
                    AreButtonsEnabled = true;
                }
                else
                {
                    OutputText += "UART GATT service not found." + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                OutputText += $"Error initializing UART GATT service. {ex.Message}" + Environment.NewLine;
            }
        }

        private async Task GetCharacteristicWithoutUUID()
        {
            try
            {
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
                            AreButtonsEnabled = true;
                        }
                    }
                    else
                    {
                        OutputText += "UART GATT service not found." + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                OutputText += $"Error initializing UART GATT service. {ex.Message}" + Environment.NewLine;
            }
        }

        private async void Send()
        {
            try
            {
                if (sendCharacteristic != null)
                {
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.ASCII.GetBytes($"{entryText}\r\n"));
                }
            }
            catch (Exception ex)
            {
                OutputText += $"Error sending comand to UART. {ex.Message}" + Environment.NewLine;
            }
        }

        private async Task Disconnect()
        {
            try
            {
                await _bluetoothAdapter.DisconnectDeviceAsync(_connectedDevice);
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch
            { // TODO 
                await PopupNavigation.Instance.PushAsync(new InfoPopup(
                            Resources.Strings.Resource.ErrorTitle,
                            "Error disconnecting to BLE device"));
            }
        }

        private async void StartReceiving()
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
            buffer.AddRange(args.Characteristic.Value);
            var t = Task.Run(() =>
            {
                var receivedBytes = args.Characteristic.Value;
                var stringValue = Encoding.ASCII.GetString(receivedBytes, 0, receivedBytes.Length);
                if (string.Equals(stringValue, "End"))
                {
                    OutputText += "End of samples";
                }
                else
                {
                    samples.Add(stringValue);
                    OutputText += stringValue + ", ";
                }
            });
            t.Wait();
        }

        private async Task GoToChartsPage()
        {
            IsBusy = true;
            List<float> floatSamples = new List<float>();
            int counter = 0;
            foreach(var sample in samples)
            {
                counter++;
                if(float.TryParse(sample, out var value))
                {
                    floatSamples.Add(value);
                }
            }
            OutputText += $"num = {counter}";
            await OpenPage(new ChartsPage(floatSamples));
            IsBusy = false;
        }
    }
}
