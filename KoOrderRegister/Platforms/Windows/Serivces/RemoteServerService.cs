
using KORCore.Modules.Remote.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KoOrderRegister.Modules.Remote.Server.Service
{
    public class RemoteServerService : IRemoteServerService
    {

        private int _port = Preferences.Get("remoteServerPort", -1);
        private string _private_key = Preferences.Get("private_key", GenerateRandomString(10));


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
            return $"{KORConnect.Program.GetLocalIPAddress()}:{_port}";
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
    }
}
