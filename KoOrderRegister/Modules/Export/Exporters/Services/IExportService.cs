using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Export.Exporters.Services
{
    public interface IExportService
    {
        Task Export(string outputPath, CancellationToken cancellationToken, Action<float> progressCallback = null);
        void CreateZip();
    }
}
