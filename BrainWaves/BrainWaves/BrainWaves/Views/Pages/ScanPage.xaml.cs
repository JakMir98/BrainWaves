
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
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
