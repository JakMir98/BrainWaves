using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions;
using Plugin.BLE;
using BrainWaves.ViewModels;

namespace BrainWaves.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanPage : ContentPage
    {
        public ScanPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as ScanViewModel).SetupAdapterAsync();
        }

        private async void DevicesList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await (BindingContext as ScanViewModel).ItemClicked(sender, e);
        }
    }
}
