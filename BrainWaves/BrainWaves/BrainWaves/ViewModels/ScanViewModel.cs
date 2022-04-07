using BrainWaves.Popups;
using BrainWaves.Views;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Rg.Plugins.Popup.Services;
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
    public class ScanViewModel : BaseViewModel
    {
        private IAdapter bluetoothAdapter;
        private ObservableCollection<IDevice> gattDevices = new ObservableCollection<IDevice>();
        private bool isScanning = false;
        private bool canScan = true;
        private string infoMessage;
        private bool isInfoVisible;

        public ICommand ScanDevicesCommand { private set; get; }
        public ICommand StopScanningCommand { private set; get; }
        public ICommand GoToSettingsCommand { private set; get; }

        public ScanViewModel()
        {
            Title = Resources.Strings.Resource.FindDevice;
            ScanDevicesCommand = new Command(async () => await ScanDevices());
            StopScanningCommand = new Command(async () => await StopScanning());
            GoToSettingsCommand = new Command(async () => await GoToSettings());

            if(!canScan)
            {
                var ble = CrossBluetoothLE.Current;
                if (ble.State == BluetoothState.Off)
                {
                    IsInfoVisible = true;
                    CanScan = false;
                    InfoMessage = Resources.Strings.Resource.BleOff;
                }
                else if (ble.State == BluetoothState.Unavailable || ble.State == BluetoothState.Unknown)
                {
                    IsInfoVisible = true;
                    CanScan = false;
                    InfoMessage = Resources.Strings.Resource.BleUnavailable;
                }
                else
                {
                    IsInfoVisible = false;
                }
            }
            
        }

        public ObservableCollection<IDevice> GattDevices
        {
            get => gattDevices;
            set => SetProperty(ref gattDevices, value);
        }

        public bool IsScanning
        {
            get => isScanning;
            set => SetProperty(ref isScanning, value);
        }

        public bool CanScan
        {
            get => canScan;
            set => SetProperty(ref canScan, value);
        }

        public string InfoMessage
        {
            get => infoMessage;
            set => SetProperty(ref infoMessage, value);
        }

        public bool IsInfoVisible
        {
            get => isInfoVisible;
            set => SetProperty(ref isInfoVisible, value);
        }

        public async Task SetupAdapterAsync()
        {
            try
            {
                bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
                bluetoothAdapter.DeviceDiscovered += (sender, foundBleDevice) =>
                {
                    if (foundBleDevice.Device != null && !string.IsNullOrEmpty(foundBleDevice.Device.Name)
                            && !gattDevices.Contains(foundBleDevice.Device))
                        GattDevices.Add(foundBleDevice.Device);
                };
            }
            catch (Exception ex)
            {
                await App.OpenInfoPopup(Resources.Strings.Resource.ErrorTitle, ex.Message);
                CanScan = false;
            }
        }

        private async Task<bool> PermissionsGrantedAsync()
        {
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (locationPermissionStatus != PermissionStatus.Granted)
            {
                var status = await Permissions.RequestAsync<Permissions.LocationAlways>();
                return status == PermissionStatus.Granted;
            }
            return true;
        }

        private async Task ScanDevices()
        {
            if(canScan)
            {
                IsBusy = true;
                IsScanning = true;
                if (!await PermissionsGrantedAsync())
                {
                    await App.OpenInfoPopup(
                        Resources.Strings.Resource.PerrmisionRequiredTitle,
                        Resources.Strings.Resource.ApplicationNeedPermissionText);
                    IsBusy = false;
                    return;
                }

                GattDevices.Clear();

                foreach (var device in bluetoothAdapter.ConnectedDevices)
                    GattDevices.Add(device);

                await bluetoothAdapter.StartScanningForDevicesAsync();
                IsScanning = false;
                IsBusy = false;
            }
        }


        public async Task ItemClicked(object sender, ItemTappedEventArgs e)
        {
            IsBusy = true;
            await StopScanning();

            IDevice selectedItem = e.Item as IDevice;

            if (selectedItem.State == DeviceState.Connected)
            {
                await OpenPage(new BluetoothDataPage(selectedItem));
            }
            else
            {
                try
                {
                    var connectParameters = new ConnectParameters(false, true);
                    await bluetoothAdapter.ConnectToDeviceAsync(selectedItem, connectParameters);
                    await OpenPage(new BluetoothDataPage(selectedItem));
                }
                catch
                {
                    await App.OpenInfoPopup(
                            Resources.Strings.Resource.ErrorTitle,
                            Resources.Strings.Resource.ApplicationNeedPermissionText + $" {selectedItem.Name ?? "N/A"}");
                }
            }
            IsBusy = false;
        }

        private async Task StopScanning()
        {
            await bluetoothAdapter.StopScanningForDevicesAsync();
            IsScanning = false;
        }

        private async Task GoToSettings()
        {
            await OpenPage(new SettingsPage());
        }
    }
}
