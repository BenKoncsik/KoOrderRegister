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

namespace KoOrderRegister.Modules.Export.Pdf.ViewModel
{
    public class ExportersViewModel : BaseViewModel
    {
        //private readonly IExcelExportService _exportService;

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
        public ExportersViewModel()
        {
            //  _exportService = exportService;
        }
        public void ProgressCallback(float precent)
        {
            LoadingTXT = $"{AppRes.Loading}: {precent}%";
        }
        public async void ExportData()
        {

        }

    }
}
