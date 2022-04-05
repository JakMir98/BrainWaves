using BrainWaves.Popups;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace BrainWaves.Services
{
    public class BluetoothService
    {
        private IAdapter bluetoothAdapter;
        private ObservableCollection<IDevice> gattDevices = new ObservableCollection<IDevice>();

        public BluetoothService()
        {

        }

        public async Task<bool> SetupAdapterAsync()
        {
            try
            {
                bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
                bluetoothAdapter.DeviceDiscovered += (sender, foundBleDevice) =>
                {
                    if (foundBleDevice.Device != null && !string.IsNullOrEmpty(foundBleDevice.Device.Name))
                        gattDevices.Add(foundBleDevice.Device);
                };
                return true;
            }
            catch (Exception ex)
            {
                await PopupNavigation.Instance.PushAsync(new InfoPopup(Resources.Strings.Resource.ErrorTitle, ex.Message));
                return false;
            }
        }

        private async Task StopScanning()
        {
            await bluetoothAdapter.StopScanningForDevicesAsync();
        }
    }
}
