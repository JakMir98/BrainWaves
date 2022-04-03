using BrainWaves.Helpers;
using BrainWaves.ViewModels;
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
        public BluetoothDataPage(IDevice connectedDevice)
        {
            InitializeComponent();
            BindingContext = new BluetoothDataViewModel(connectedDevice);
        }
    }
}