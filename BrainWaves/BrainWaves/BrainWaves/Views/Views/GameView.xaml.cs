using BrainWaves.ViewModels.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.Views.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameView : ContentView
    {
        public GameView()
        {
            InitializeComponent();
            BindingContext = new GameViewModel();
        }
    }
}