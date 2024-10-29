using KORCore.Modules.Database.Factory;
using KORCore.Modules.Database.Services;
using KORCore.Modules.Remote.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Remote.Client.Service
{
    public class RemoteClientService : IRemoteClientService
    {
        #region DI
        private readonly RemoteDatabaseModel _clientDatabaseModel;
        private readonly IDatabaseModelFactory _databaseModelFactory;
        #endregion
        public RemoteClientService(RemoteDatabaseModel databaseModel, IDatabaseModelFactory databaseModelFactory) 
        {
            _clientDatabaseModel = databaseModel;
            _databaseModelFactory = databaseModelFactory;
        }
        public async Task<bool> ConnectAsync(ConnectionData connectionData)
        {
            try
            {
                RemoteDatabaseModel.SetUrl(connectionData.Url);
                _databaseModelFactory.SetDatabase(true);
                return true;
            }catch(System.Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> DisconnectAsync()
        {
            try
            {
                RemoteDatabaseModel.SetUrl(string.Empty);
                _databaseModelFactory.SetDatabase(false);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
    }
}
