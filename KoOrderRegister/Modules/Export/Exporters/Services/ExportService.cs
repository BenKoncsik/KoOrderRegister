using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Export.Exporters.Services
{
    public class ExportService
    {
        public ExportService()
        {
                  
        }

        public async Task ExportData(IExportService exportService, bool IsCreateZip, Action<float> progressCallback)
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            try
            {
                var result = await FolderPicker.PickAsync(cancellationToken.Token);
                if (result != null && result.IsSuccessful && !string.IsNullOrEmpty(result.Folder.Path))
                {
                    var fullPath = Path.Combine(result.Folder.Path);
                    await exportService.Export(fullPath, cancellationToken.Token, progressCallback);
                    if (IsCreateZip)
                    {
                        exportService.CreateZip();
                    }
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
