using CommunityToolkit.Maui.Storage;
using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Export.Types.Excel.Services;
using KoOrderRegister.Modules.Export.Types.Services;
using KoOrderRegister.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.Export.ViewModel
{
    public class ExportersViewModel : BaseViewModel
    {
        private readonly IExcelExportService _exportService;

        #region Binding varribles
        private bool _isCreateZip = false;
        public bool IsCreateZip
        {
            get => _isCreateZip;
            set
            {
                if (value != _isCreateZip)
                {
                    _isCreateZip = value;
                    OnPropertyChanged(nameof(IsCreateZip));
                }
            }
        }
        #endregion
        #region Commands
        public ICommand ExportDataCommand => new Command(ExportData);
        #endregion
        public ExportersViewModel(IExcelExportService exportService)
        {
            _exportService = exportService;
        }
        public void ProgressCallback(float precent)
        {
            LoadingTXT = $"{AppRes.Loading}: {precent}%";
        }
        public async void ExportData()
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
                    if (IsCreateZip)
                    {
                        _exportService.CreateZip();
                    }
                    
                    IsRefreshing = false;
                }
            }
            finally
            {
                cancellationToken.Cancel();
                cancellationToken.Dispose();
            }
        }

    }
}
