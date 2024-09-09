using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Database.Models;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace KoOrderRegister.Modules.Customer.ViewModels
{
    public class PersonDetailPopUp : INotifyPropertyChanged
    {
        private CustomerModel _customer = new CustomerModel();
        public CustomerModel Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands
        public ICommand CloseCommand => new Command(OnClose);
        public Command<string> CopyCommand => new Command<string>(Copy);
        #endregion

        public async void Copy(string txt)
        {
            await Clipboard.SetTextAsync(txt);
            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(AppRes.Copy, $"{AppRes.Copyd}: {txt}", AppRes.Ok);
        }
        public async void OnClose()
        {
            await MopupService.Instance.PopAsync();
        }
    }
}
