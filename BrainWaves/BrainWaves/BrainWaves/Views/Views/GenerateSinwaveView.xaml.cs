using BrainWaves.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.Views.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GenerateSinwaveView : ContentView
    {
        public GenerateSinwaveView()
        {
            InitializeComponent();
            BindingContext = new GenerateSinwaveViewModel();
        }
    }
}