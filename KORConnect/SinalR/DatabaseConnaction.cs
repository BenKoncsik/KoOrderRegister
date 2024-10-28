using KORCore.Modules.Database.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace KORConnect.SinalR
{
    public class DatabaseConnaction : Hub
    {
        #region DI
        private readonly IDatabaseModel _databaseModel;
        #endregion
        public DatabaseConnaction(IDatabaseModel databaseModel)
        {
            _databaseModel = databaseModel;
        }

     
        public async Task TriggerDatabaseChange(string action, string jsonData)
        {
            Debug.WriteLine($"TriggerDatabaseChange: {action} - {jsonData}");
            await Clients.Others.SendAsync("DatabaseChanged", action, jsonData);
        }

        public async Task TriggerDatabseChangedResult(string action, string jsonData)
        {
            Debug.WriteLine($"TriggerDatabseChangedResult: {action} - {jsonData}");
            await Clients.Others.SendAsync("DatabaseChangedResult", action, jsonData);
        }

    }
}
