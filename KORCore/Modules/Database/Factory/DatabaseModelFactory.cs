using KORCore.Modules.Database.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Factory
{
    public class DatabaseModelFactory : IDatabaseModelFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private bool _isRemote = false;
        public DatabaseModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void SetDatabase(bool isRemote)
        {
            _isRemote = isRemote;
        }
        public IDatabaseModel Get()
        {
            if (_isRemote)
            {
                return _serviceProvider.GetRequiredService<RemoteDatabaseModel>();
            }
            else
            {
                return _serviceProvider.GetRequiredService<DatabaseModel>();
            }
        }
    }
}
