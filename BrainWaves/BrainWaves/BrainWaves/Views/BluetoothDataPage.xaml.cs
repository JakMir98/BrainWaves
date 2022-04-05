using BrainWaves.ViewModels;
using Plugin.BLE.Abstractions.Contracts;
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