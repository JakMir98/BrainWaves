using BrainWaves.Helpers;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothDataPage : ContentPage
    {
        private readonly IDevice _connectedDevice;
        private readonly IAdapter _bluetoothAdapter;
        private ICharacteristic sendCharacteristic;
        private ICharacteristic receiveCharacteristic;

        public BluetoothDataPage(IDevice connectedDevice)
        {
            InitializeComponent();
            _connectedDevice = connectedDevice;
            _bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            StartReceivingButton.IsEnabled = false;
            StopReceivingButton.IsEnabled = false;
            SendCommadButton.IsEnabled = false;
            //InitButton.IsEnabled = !(ScanButton.IsEnabled = false);
        }

        private async void InitalizeCommandButton_Clicked(object sender, EventArgs e)
        {
            await GetCharacteristicWithoutUUID();
        }

        private async Task GetCharacteristic()
        {
            try
            {
                var service = await _connectedDevice.GetServiceAsync(Constants.UartGattServiceId);

                if (service != null)
                {
                    sendCharacteristic = await service.GetCharacteristicAsync(Constants.GattCharacteristicReceiveId);
                    receiveCharacteristic = await service.GetCharacteristicAsync(Constants.GattCharacteristicSendId);

                    Output.Text = $"Send: read:{sendCharacteristic.CanRead} write:{sendCharacteristic.CanWrite} update:{sendCharacteristic.CanUpdate}\n" +
                        $"Receive: read:{receiveCharacteristic.CanRead} write:{receiveCharacteristic.CanWrite} update:{receiveCharacteristic.CanUpdate}\n";

                    StartReceivingButton.IsEnabled = true;
                    StopReceivingButton.IsEnabled = true;
                    SendCommadButton.IsEnabled = true;
                }
                else
                {
                    Output.Text += "UART GATT service not found." + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                Output.Text += $"Error initializing UART GATT service. {ex.Message}" + Environment.NewLine;
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
                            if(characteristic.CanWrite)
                            {
                                sendCharacteristic = characteristic;
                            }
                            else if(characteristic.CanUpdate)
                            {
                                receiveCharacteristic = characteristic;
                            }
                            StartReceivingButton.IsEnabled = true;
                            StopReceivingButton.IsEnabled = true;
                            SendCommadButton.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Output.Text += "UART GATT service not found." + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                Output.Text += $"Error initializing UART GATT service. {ex.Message}" + Environment.NewLine;
            }
        }

        private async void SendCommandButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sendCharacteristic != null)
                {
                    var bytes = await sendCharacteristic.WriteAsync(Encoding.ASCII.GetBytes($"{CommandTxt.Text}\r\n"));
                }
            }
            catch(Exception ex)
            {
                Output.Text += $"Error sending comand to UART. {ex.Message}" + Environment.NewLine;
            }
        }

        private async void DisconnectButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await _bluetoothAdapter.DisconnectDeviceAsync(_connectedDevice);
                await Navigation.PopAsync();
            }
            catch
            {
                await DisplayAlert("Error disconnecting", $"Error disconnecting to BLE device:", "OK");
            }
        }

        private async void StartReceivingButton_Clicked(object sender, EventArgs e)
        {
            if (receiveCharacteristic != null)
            {
                receiveCharacteristic.ValueUpdated += ReadValues;

                await receiveCharacteristic.StartUpdatesAsync();
                InitButton.IsEnabled = !(SendCommadButton.IsEnabled = true);
            }
        }

        private void StopReceivingButton_Clicked(object sender, EventArgs e)
        {
            receiveCharacteristic.ValueUpdated -= ReadValues;
        }

        private void ReadValues(object o, CharacteristicUpdatedEventArgs args)
        {
            var receivedBytes = args.Characteristic.Value;
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                Output.Text += Encoding.UTF8.GetString(receivedBytes, 0, receivedBytes.Length) + Environment.NewLine;
            });
        }

        private async void ChartsButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChartsPage());
        }
    }
}