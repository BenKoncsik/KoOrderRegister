using Camera.MAUI;
using KoOrderRegister.Modules.Remote.Server.Service;
using KoOrderRegister.ViewModel;
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
        //private bool _isRemoteServer => _remoteServerService.IsEnable;
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
        public ImageSource RemoteServerQrCode => IsRemoteServer ? "remote_server_on" : "remote_server_off";
        public string _remoteServerUrl { get; set; }
        public string RemoteServerUrl
        {
            get => _remoteServerUrl;
            set
            {
                if (value != _remoteServerUrl)
                {
                    _remoteServerUrl = value;
                    OnPropertyChanged(nameof(RemoteServerUrl));
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
            string url = await _remoteServerService.GetRemoteServerIP();
            Debug.WriteLine($"Url: {url}");
            if (string.IsNullOrEmpty(url)) return;
            RemoteServerUrl = url;
            IsRefreshing = false;
        }

        internal void OnAppearing()
        {
            IsRemoteServer = _remoteServerService.IsEnable;
        }
    }
}
