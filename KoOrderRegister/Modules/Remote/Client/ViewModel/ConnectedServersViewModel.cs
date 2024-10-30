using DocumentFormat.OpenXml.VariantTypes;
using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Remote.Client.Service;
using KoOrderRegister.ViewModel;
using KORCore.Modules.Database.Factory;
using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using KORCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.Remote.Client.ViewModel
{
    public class ConnectedServersViewModel : BaseViewModel
    {
        #region DI
        private readonly ILocalDatabase _localDatabase;
        private readonly IRemoteDatabase _remoteDatabase;
        private readonly IDatabaseModelFactory _databaseModelFactory;
        private readonly IRemoteClientService _remoteClientService;
        #endregion
        #region Binding varrible
        public ObservableCollection<ConnectionDeviceData> Connections { get; set; } = new ObservableCollection<ConnectionDeviceData>();
        public string SearchTXT { get; set; } = string.Empty;
        #endregion
        #region Commands
        public ICommand UpdateCommand { get; }
        public Command<string> SearchCommand { get; }
        public Command<ConnectionDeviceData> DeleteConnectionCommand { get; }
        public Command<ConnectionDeviceData> ConnectServerCommand { get; }
        public Command<ConnectionDeviceData> DisconetionServerCommand { get; }
        public Command<ConnectionDeviceData> ConnectTapServerCommand { get; }
        #endregion

        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public ConnectedServersViewModel(ILocalDatabase localDatabase, IRemoteDatabase remoteDatabase, IRemoteClientService remoteClientService, IDatabaseModelFactory databaseModelFactory)
        {
            _localDatabase = localDatabase;
            _remoteDatabase = remoteDatabase;
            _databaseModelFactory = databaseModelFactory;
            _remoteClientService = remoteClientService;
            UpdateCommand = new Command(Update);
            SearchCommand = new Command<string>(Search);
            DeleteConnectionCommand = new Command<ConnectionDeviceData>(Delete);
            ConnectServerCommand = new Command<ConnectionDeviceData>(Connection);
            DisconetionServerCommand = new Command<ConnectionDeviceData>(Disconection);
            ConnectTapServerCommand = new Command<ConnectionDeviceData>(ConectedTap);
        }

        private async void Update()
        {
            using(new LowPriorityTaskManager())
            {
                if (Connections != null)
                {
                    Connections.Clear();
                }
                else
                {
                    Connections = new ObservableCollection<ConnectionDeviceData>();
                }

                await foreach (var connection in _localDatabase.GetConnectionDataAsStreamAsync(_cancellationToken.Token))
                {
                    ConnectionDeviceData? findCon = Connections.FirstOrDefault(c => c.Id.Equals(connection.Id));
                    if (findCon != null)
                    {
                        Connections.Remove(findCon);
                    }
                    MainThread.BeginInvokeOnMainThread(() => Connections.Add(connection));

                }
            }
        }

        public async void Search(string txt)
        {
            IsRefreshing = true;
            using(new LowPriorityTaskManager())
            {
                try
                {
                    await foreach (ConnectionDeviceData connection in _localDatabase.SearchConnectionDataAsStreamAsync(txt, _cancellationToken.Token))
                    {
                        if (!Connections.Any(c => c.Id.Equals(connection.Id)))
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Connections?.Add(connection);
                            });
                        }
                    }
                }
                catch (OperationCanceledException)
                {

                }
                finally
                {
                    IsRefreshing = false;
                }
            }
            
        }
        private async void Delete(ConnectionDeviceData connection)
        {
            bool result = await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.AreYouSureYouWantToDelete, AppRes.Yes, AppRes.No);
            if (result)
            {
                int deleteResult = await _localDatabase.DeleteConnection(connection);
                if (deleteResult > 0)
                {
                    Connections.Remove(connection);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(AppRes.Delete, AppRes.FailedToDelete, AppRes.Ok);
                }
            }
        }

        private async void Connection(ConnectionDeviceData deviceData)
        {
            bool result = await Application.Current.MainPage.DisplayAlert(AppRes.Connection, AppRes.AreYouSureYouWantToConnection + " " + deviceData.Url, AppRes.Yes, AppRes.No);
            if (result)
            {
                _remoteDatabase.SetUrl(deviceData.Url);
                _databaseModelFactory.SetDatabase(true);
                await Application.Current.MainPage.DisplayAlert(AppRes.Connection, AppRes.SuccessToConnection + " " + deviceData.Url, AppRes.Ok);
            }
        }
        private async void Disconection(ConnectionDeviceData deviceData)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool result = await Application.Current.MainPage.DisplayAlert(AppRes.Disconecting, AppRes.AreYouSureYouWantToDisconnection + " " + deviceData.Url, AppRes.Yes, AppRes.No);
                if (result)
                {
                    await _remoteClientService.DisconnectAsync();
                }
            });
        }

        private async void ConectedTap(ConnectionDeviceData deviceData)
        {
            if(_remoteDatabase.GetConectedUrl().Equals(deviceData.Url + "/api"))
            {
                Disconection(deviceData);
            }
            else
            {
                Connection(deviceData);
            }
        }

        public override void OnAppearing()
        {
            Update();
            base.OnAppearing(); 
        }
    }
}
