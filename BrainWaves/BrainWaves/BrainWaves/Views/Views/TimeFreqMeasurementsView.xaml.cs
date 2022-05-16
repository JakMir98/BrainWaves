using BrainWaves.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.Views.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeFreqMeasurementsView : ContentView
    {
        public TimeFreqMeasurementsView()
        {
            InitializeComponent();
            BindingContext = new TimeFreqMeasurementsViewModel();
        }
    }
}