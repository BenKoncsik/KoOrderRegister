using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Services
{
    public interface IVersionService
    {
        string AppVersion { get; }
        string DeviceType { get; }
        string UpdatePackageName { get; }
    }
}
