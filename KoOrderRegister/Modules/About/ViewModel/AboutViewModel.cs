using KoOrderRegister.ViewModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.About.ViewModel
{
    public class AboutViewModel : BaseViewModel
    {
        #region Commands
        public ICommand OpenUrlCommand => new Command<string>(OpenUrl);
        #endregion


        public AboutViewModel() 
        {
        
        }

        private async void OpenUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
