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
        private readonly KORConnect.Program _server;
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
        public async Task<string> GetRemoteServerIP()
        {
            return $"{KORConnect.Program.GetLocalIPAddress()}:{_port}";
        }

        public async Task<int> GetRemoteServerPort()
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
    }
}
