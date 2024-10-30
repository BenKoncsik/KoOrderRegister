using KORCore.Modules.Database.Models;
using KORCore.Modules.Remote.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Utility
{
    public static class ConnectionUtility
    {
        public static ConnectionDeviceData ToConnectionDeviceData(this ConnectionData connectionData, string deviceKey)
        {
            return new ConnectionDeviceData()
            {
                FirstConnectionData = DateTime.UtcNow,
                LastConnectionData = DateTime.UtcNow,
                DeviceKey = deviceKey,
                ServerKey = connectionData.ServerKey,
                Url = connectionData.Url
            };
        }
    }
}
