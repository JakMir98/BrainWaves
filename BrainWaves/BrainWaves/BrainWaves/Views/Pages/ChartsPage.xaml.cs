using BrainWaves.ViewModels;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartsPage : ContentPage
    {
        public ChartsPage()
        {
            InitializeComponent();
            BindingContext = new ChartsViewModel();
        }

        public ChartsPage(List<double> _samples)
        {
            InitializeComponent();
            BindingContext = new ChartsViewModel(_samples);
        }

        public ChartsPage(List<double> _samples, int samplingFreq)
        {
            InitializeComponent();
            BindingContext = new ChartsViewModel(_samples, samplingFreq);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as ChartsViewModel).DelayedFreqChartLoad();
        }
    }
}