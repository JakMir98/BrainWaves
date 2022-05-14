using BrainWaves.ViewModels;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothDataPage : ContentPage
    {
        /// <summary>
        /// Just for testing purpose
        /// </summary>
        public BluetoothDataPage()
        {
            InitializeComponent();
            BindingContext = new BluetoothDataViewModel();
        }

        public BluetoothDataPage(IDevice connectedDevice)
        {
            InitializeComponent();
            BindingContext = new BluetoothDataViewModel(connectedDevice);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as BluetoothDataViewModel).SetupCharacteristicAsync();
        }
    }
}