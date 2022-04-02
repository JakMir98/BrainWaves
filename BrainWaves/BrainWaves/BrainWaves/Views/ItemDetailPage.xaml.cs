using BrainWaves.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace BrainWaves.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}