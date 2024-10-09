using KoOrderRegister.Modules.Export.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Export.Excel.Services
{
    public interface IExcelExportService : IExportService
    {
        void CreateZip();
    }
}
