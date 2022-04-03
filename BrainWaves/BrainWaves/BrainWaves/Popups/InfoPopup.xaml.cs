using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace BrainWaves.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPopup : PopupPage
    {
        public InfoPopup(string title, string description)
        {
            InitializeComponent();
            Label_Title.Text = title;
            Label_Description.Text = description;
        }

        private void BTN_OK_Clicked(System.Object sender, System.EventArgs e)
        {
            App.ClosePopup();
        }

        protected override bool OnBackButtonPressed()
        {
            App.ClosePopup();
            return true;
        }

        protected override bool OnBackgroundClicked()
        {
            App.ClosePopup();
            return true;
        }
    }
}