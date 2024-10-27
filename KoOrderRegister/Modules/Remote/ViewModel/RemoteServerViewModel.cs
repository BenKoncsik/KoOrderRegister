using Camera.MAUI;
using KoOrderRegister.Modules.Remote.Server.Service;
using KoOrderRegister.ViewModel;
using KORCore.Modules.Remote.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Remote.ViewModel
{
    public class RemoteServerViewModel : BaseViewModel
    {
        #region DI
        private readonly IRemoteServerService _remoteServerService;
        #endregion
        #region Binding varribles
        public bool IsRemoteServer
        {
            get => _remoteServerService.IsEnable;
            set
            {
                if (_remoteServerService.IsEnable != value)
                {
                    _remoteServerService.IsEnable = value;
                    OnPropertyChanged(nameof(IsRemoteServer));
                    _remoteServerService.IsEnable = value;
                    StartStopRemoteServer(value);
                }
            }
        }
        public string _connectionString { get; set; }
        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                if (value != _connectionString)
                {
                    _connectionString = value;
                    OnPropertyChanged(nameof(ConnectionString));
                }
            }
        }
     
        #endregion
        #region Commands

        #endregion

        public RemoteServerViewModel(IRemoteServerService remoteServerService)
        {
            _remoteServerService = remoteServerService;
            IsRemoteServer = _remoteServerService.IsEnable;
            if (IsRemoteServer)
            {
                CreateImage();
            }

        }

        private async void StartStopRemoteServer(bool start)
        {
            IsRefreshing = true;
            if (start)
            {
                if(!await _remoteServerService.Start())
                {
                    IsRemoteServer = false;
                }
                await CreateImage();
            }
            else
            {
                if(!await _remoteServerService.Stop())
                {
                    IsRemoteServer = true;
                }
            }
            IsRefreshing = false;
        }
        private async Task CreateImage()
        {
            IsRefreshing = true;
            string url = _remoteServerService.ConnectionData().ToBase64();
            Debug.WriteLine($"Url: {url}");
            if (string.IsNullOrEmpty(url)) return;
            ConnectionString = url;
            IsRefreshing = false;
        }

        internal void OnAppearing()
        {
            IsRemoteServer = _remoteServerService.IsEnable;
        }
    }
}
