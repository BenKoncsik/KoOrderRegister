using KORCore.Modules.Database.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Factory
{
    public interface IDatabaseModelFactory
    {
        void SetDatabase(bool isRemote);
        IDatabaseModel Get();
    }
}
