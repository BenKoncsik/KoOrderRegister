using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace KORConnect.SinalR
{
    public class DatabaseConnaction : Hub
    {

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
