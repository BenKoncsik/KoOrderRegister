using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Services
{
    public interface IRemoteDatabase: IDatabaseModel
    {
        void SetUrl(string url);
        string GetConnectedUrl();
    }
}
