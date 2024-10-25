using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Remote.Server.Service
{
    public interface IRemoteServerService
    {
        Task<bool> Start();
        Task<bool> Stop();
        Task<int> GetRemoteServerPort();
        Task<string> GetRemoteServerIP();
        void Init();
        bool IsEnable { get; set; }
    }
}
