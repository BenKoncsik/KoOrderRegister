using CommunityToolkit.Maui.Storage;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Export.Types.Excel.Services;
using KoOrderRegister.Services;
using KoOrderRegister.ViewModel;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Application = Microsoft.Maui.Controls.Application;

namespace KoOrderRegister.Modules.BetaFunctions.ViewModels
{
    public class BetaFunctionsViewModel : BaseViewModel
    {
        private readonly IExcelExportService _exportService;
        private readonly ILocalNotificationService _notifyService;
        #region Binding varribles

        #endregion
        #region Commands
        public ICommand ExportDataCommand => new Command(ExportDataToExcel);
        public ICommand NotificationCommand => new Command(NotificationTest);
        #endregion

        public BetaFunctionsViewModel(IExcelExportService exportService, ILocalNotificationService notificationService)
        {
            _exportService = exportService;
            _notifyService = notificationService;
            _notifyService.NotificationReceived += OnNotificationReceived;
        }
        public void ProgressCallback(float precent)
        {
            LoadingTXT = $"{AppRes.Loading}: {precent}%";
        }


        public async void ExportDataToExcel()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            try
            {
                var result = await FolderPicker.PickAsync(cancellationToken.Token);
                if (result != null && result.IsSuccessful && !string.IsNullOrEmpty(result.Folder.Path))
                {
                    IsRefreshing = true;
                    var fullPath = Path.Combine(result.Folder.Path);
                    await _exportService.Export(fullPath, cancellationToken.Token, ProgressCallback);
                    _exportService.CreateZip();
                    IsRefreshing = false;
                }
            }
            finally
            {
                cancellationToken.Cancel();
                cancellationToken.Dispose();
            }
        }
        #region Notification
        private int id = -1;
        public async void NotificationTest()
        {
            if(id == -1)
            {
                id = _notifyService.SendNotification("Test", "Test");
            }
            else
            {
                _notifyService.UpdateNotification(id, $"Test: {id}", "Test");
            }
           
        }
        private async void OnNotificationReceived(NotificationActionArgs e)
        {
            if (e.Id == id)
            {
                await Application.Current.MainPage.DisplayAlert(AppRes.BetaFuctions, AppRes.UpdateDownloaded, AppRes.Open, AppRes.Cancel);
            }
        }

        #endregion
    }
}
