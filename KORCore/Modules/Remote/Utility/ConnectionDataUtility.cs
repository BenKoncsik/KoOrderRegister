using KORCore.Modules.Remote.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Remote.Utility
{
    public static class ConnectionDataUtility
    {
        public static string ToBase64(this ConnectionData connectionData)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{connectionData.Url}|{connectionData.ServerKey}|{connectionData.Version}"));
        }

        public static ConnectionData ToConnantionData(this string base64)
        {
            var data = Encoding.UTF8.GetString(Convert.FromBase64String(base64)).Split('|');
            return new ConnectionData
            {
                Url = data[0],
                ServerKey = data[1],
                Version = data[2]
            };
        }
    }
}
