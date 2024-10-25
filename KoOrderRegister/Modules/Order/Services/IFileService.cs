using KORCore.Module.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Order.Services
{
    public interface IFileService
    {
        Task<bool> SaveFileToLocal(FileModel file);
        Task<string> SaveFileToTmp(FileModel file);
        Task<string> CalculateHashAsync(byte[] content);

    }
}
