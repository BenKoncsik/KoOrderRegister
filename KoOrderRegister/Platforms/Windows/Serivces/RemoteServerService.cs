using Microsoft.AspNetCore.SignalR.Client;
using KoOrderRegister.Modules.Database.Services;
using KORCore.Modules.Remote.Model;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KORCore.Modules.Database.Utility;
using KORCore.Modules.Database.Models;


namespace KoOrderRegister.Modules.Remote.Server.Service
{
    public class RemoteServerService : IRemoteServerService
    {
        #region DI
        private readonly IDatabaseModel _databaseModel;
        #endregion

        private int _port = Preferences.Get("remoteServerPort", -1);
        private string _private_key = Preferences.Get("private_key", GenerateRandomString(10));
        private HubConnection _hubConnection;
        
        public RemoteServerService(IDatabaseModel databaseModel)
        {
            _databaseModel = databaseModel; 
        }

        public bool IsEnable 
        { 
            get => Preferences.Get("remoteServer", false);
            set => Preferences.Set("remoteServer", value);
        }
        public async Task<bool> Start()
        {
            try
            {
                KORConnect.Program.Main(new string[0]);
                _port = KORConnect.Program.CreateAndRunWebHost(new string[0], (_port == -1)? null : _port);
                Preferences.Set("remoteServerPort", _port);
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(GetRemoteServerIP() + "/databaseHub")
                    .Build();
                IDatabaseModel.OnDatabaseChange += OnDatabaseChange;
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Error start service: {ex}");
                return false;
            }

        }

        public async Task<bool> Stop()
        {
            try
            {
                await KORConnect.Program.StopWebHost();
                IDatabaseModel.OnDatabaseChange -= OnDatabaseChange;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stop service: {ex}");
                return false;
            }
        }
        public string GetRemoteServerIP()
        {
            string url = $"http://{KORConnect.Program.GetLocalIPAddress()}:{_port}";
            Debug.WriteLine($"URL: {url}");
            return url;
        }

        public int GetRemoteServerPort()
        {
            return _port;
        }

        public async void Init()
        {
            if (IsEnable)
            {
                await Start();
            }
        }
        public ConnectionData ConnectionData()
        {
            return new ConnectionData()
            {
                Url = GetRemoteServerIP(),
                ServerKey = _private_key,
                Version = AppShell.AppVersion
            };
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }

        private async void OnDatabaseChange(string action, object data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            Debug.WriteLine($"Remote database changed: {action}");
            try
            {
                await _hubConnection.InvokeAsync("TriggerDatabaseChange", action, jsonData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending data: {ex.Message}");
            }
        }

        private async void TestDatabse(string action, string jsonData)
        {
            _hubConnection.On<string, string>("DatabaseChanged",async (action, jsonData) =>
            {
                var data = JsonConvert.DeserializeObject<object>(jsonData);
                Debug.WriteLine($"Action: {action}, Data: {data}");
                object result = string.Empty;
                switch(action)
                {
                    case DatabaseChangedType.CUSTOMER_CREATED:
                        CustomerModel customer = JsonConvert.DeserializeObject<CustomerModel>(jsonData);
                        result = await _databaseModel.CreateCustomer(customer);
                        break;
                    case DatabaseChangedType.CUSTOMER_UPDATED:
                        CustomerModel updatedCustomer = JsonConvert.DeserializeObject<CustomerModel>(jsonData);
                        result = await _databaseModel.UpdateCustomer(updatedCustomer);
                        break;
                    case DatabaseChangedType.CUSTOMER_DELETED:
                        CustomerModel deletedCustomer = JsonConvert.DeserializeObject<CustomerModel>(jsonData);
                        result = await _databaseModel.DeleteCustomer(Guid.Parse(deletedCustomer.Id));
                        break;
                    case DatabaseChangedType.CUSTOMER_RETRIEVED:
                        string retrievedCustomer = JsonConvert.DeserializeObject<string>(jsonData);
                        result = _databaseModel.GetCustomerById(Guid.Parse(retrievedCustomer)); 
                        break;
                    case DatabaseChangedType.CUSTOMERS_RETRIEVED:
                        result = _databaseModel.GetAllCustomers(); 
                        break;
                    case DatabaseChangedType.CUSTOMER_STREAM_RETRIEVED:
                        result = _databaseModel.GetAllCustomersAsStream(new CancellationToken());
                        break;

                }
                Debug.WriteLine(JsonConvert.SerializeObject(result));
                await _hubConnection.InvokeAsync("TriggerDatabseChangedResult", action, JsonConvert.SerializeObject(result));
            });
        }
    }
}
