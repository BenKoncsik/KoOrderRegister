using KORCore.Modules.Database.Factory;
using KORCore.Modules.Database.Services;
using KORCore.Modules.Database.Utility;
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
        private readonly IRemoteDatabase _remoteDatabaseModel;
        private readonly ILocalDatabase _localDatabaseModel;
        private readonly IDatabaseModelFactory _databaseModelFactory;
        #endregion
        private string _deviceKey = Preferences.Get("device_key", new Guid().ToString());
        public RemoteClientService(IRemoteDatabase databaseModel, ILocalDatabase localDatabase, IDatabaseModelFactory databaseModelFactory) 
        {
            _remoteDatabaseModel = databaseModel;
            _localDatabaseModel = localDatabase;
            _databaseModelFactory = databaseModelFactory;
        }
       
        public async Task<bool> ConnectAsync(ConnectionData connectionData)
        {
            try
            {
                _remoteDatabaseModel.SetUrl(connectionData.Url);
                _databaseModelFactory.SetDatabase(true);
                await _localDatabaseModel.CreateOrUpdateDatabaseConnection(connectionData.ToConnectionDeviceData(_deviceKey));
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
                _remoteDatabaseModel.SetUrl(string.Empty);
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
