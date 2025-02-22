using KORCore.Modules.Remote.Model;
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
        int GetRemoteServerPort();
        string GetRemoteServerIP();
        ConnectionData ConnectionData();
        
        void Init();
        bool IsEnable { get; set; }
    }
}
