using KORCore.Modules.Remote.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Remote.Client.Service
{
    public interface IRemoteClientService
    {
        Task<bool> ConnectAsync(ConnectionData connectionData);
        Task<bool> DisconnectAsync();
    }
}
