using BrainWaves.Models;
using BrainWaves.ViewModels;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WavesPage : ContentPage
    {
        public WavesPage()
        {
            InitializeComponent();
        }

        public WavesPage(List<BrainWaveSample> samples)
        {
            InitializeComponent();
            BindingContext = new WavesViewModel(samples);
        }
    }
}