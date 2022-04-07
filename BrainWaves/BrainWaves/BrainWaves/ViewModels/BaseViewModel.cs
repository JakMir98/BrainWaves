using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrainWaves.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool isBusy = false;
        private string busyMessage = string.Empty;
        string title = string.Empty;
        public ICommand GoBackCommand { protected set; get; }
        
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public string BusyMessage
        {
            get { return busyMessage; }
            set { SetProperty(ref busyMessage, value); }
        }
        
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        protected async Task OpenPage(Page page)
        {
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        protected async Task GoBack()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
