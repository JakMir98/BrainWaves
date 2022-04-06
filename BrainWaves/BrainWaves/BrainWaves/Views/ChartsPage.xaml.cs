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
        }

        public ChartsPage(List<float> _samples)
        {
            InitializeComponent();
            BindingContext = new ChartsViewModel(_samples);
        }
    }
}