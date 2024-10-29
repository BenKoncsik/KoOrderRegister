using Camera.MAUI;
using Camera.MAUI.ZXingHelper;
using KoOrderRegister.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Camera.MAUI.ZXingHelper;
using CommunityToolkit.Maui.Views;
using static Camera.MAUI.CameraView;
using ZXing;
using KoOrderRegister.Utility;
using KORCore.Modules.Remote.Exception;
using KORCore.Modules.Remote.Model;
using KORCore.Modules.Remote.Utility;
using KoOrderRegister.Modules.Remote.Client.Service;
using System.Windows.Input;
using Plugin.LocalNotification;
using KoOrderRegister.Services;
using KoOrderRegister.Localization;

namespace KoOrderRegister.Modules.Remote.Client.ViewModel
{

    public class ClientConnectionViewModel : BaseViewModel
    {
        #region DI
        private readonly IRemoteClientService _remoteClientService;
        private readonly ILocalNotificationService _notificationService;
        #endregion
        #region Commands
        public ICommand ConnectionCommand => new Command(StartConnection);
        #endregion
        #region Binding varrible
        private CameraInfo camera = null;
        public CameraInfo Camera
        {
            get => camera;
            set
            {
                camera = value;
                OnPropertyChanged(nameof(Camera));
                AutoStartPreview = false;
                OnPropertyChanged(nameof(AutoStartPreview));
                AutoStartPreview = true;
                OnPropertyChanged(nameof(AutoStartPreview));
            }
        }
        private ObservableCollection<CameraInfo> cameras = new();
        public ObservableCollection<CameraInfo> Cameras
        {
            get => cameras;
            set
            {
                cameras = value;
                OnPropertyChanged(nameof(Cameras));
            }
        }
        public int NumCameras
        {
            set
            {
                if (value > 0)
                    Camera = Cameras.First();
            }
        }
        public BarcodeDecodeOptions BarCodeOptions { get; set; } = new Camera.MAUI.ZXingHelper.BarcodeDecodeOptions
                                                                    {
                                                                        AutoRotate = true,
                                                                        PossibleFormats = { BarcodeFormat.QR_CODE
                                                                    },
                                                                        ReadMultipleCodes = false,
                                                                        TryHarder = true,
                                                                        TryInverted = true
                                                                    };
        private ConnectionData connectionData = new ConnectionData();
        public ConnectionData ConnectionData
        {
            get => connectionData;
            set
            {
                if (!connectionData.Equals(value))
                {
                    connectionData = value;
                    OnPropertyChanged(nameof(ConnectionData));
                    StartConnection();
                }
                
            }
        }
        private string _barcodText = string.Empty;
        public string BarcodeText 
        {
            get => _barcodText;
            set
            {
                if (!value.Equals(_barcodText))
                {
                    try
                    {
                        
                        ConnectionData = value.ToConnentionData();
                        DeviceFeature.VibrationOn();
                        _barcodText = value;
                        Debug.WriteLine($"Detected text: {value}");
                        OnPropertyChanged(nameof(BarcodeText));
                    }
                    catch(InvalidConnectionDataExcaption)
                    {
                        DeviceFeature.VibrationOn(1000);
                        Debug.WriteLine($"Detected text: {value}");
                    }
                    
                }
                
            }
        }
        public bool AutoStartPreview { get; set; } = false;
        public bool AutoStartRecording { get; set; } = false;
        private Result[] barCodeResults;
        public Result[] BarCodeResults
        {
            get => barCodeResults;
            set
            {
                barCodeResults = value;
                if (barCodeResults != null && barCodeResults.Length > 0)
                {
                    BarcodeText = barCodeResults[0].Text;
                }
                
            }
        }
        private bool takeSnapshot = false;
        public bool TakeSnapshot
        {
            get => takeSnapshot;
            set
            {
                takeSnapshot = value;
                OnPropertyChanged(nameof(TakeSnapshot));
            }
        }
        public float SnapshotSeconds { get; set; } = 0f;
        public string Seconds
        {
            get => SnapshotSeconds.ToString();
            set
            {
                if (float.TryParse(value, out float seconds))
                {
                    SnapshotSeconds = seconds;
                    OnPropertyChanged(nameof(SnapshotSeconds));
                }
            }
        }
        public event BarcodeResultHandler BarcodeDetected;
        #endregion

        public ClientConnectionViewModel(IRemoteClientService remoteClientService, ILocalNotificationService notificationService)
        {
            _remoteClientService = remoteClientService;
            _notificationService = notificationService;
        }

        private async void StartConnection()
        {
            try
            {
                if(await _remoteClientService.ConnectAsync(ConnectionData))
                {
                    _notificationService.SendNotification(AppRes.Connection, AppRes.Ok);
                }
                else
                {
                    _notificationService.SendNotification(AppRes.Connection, AppRes.Failed);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Connection error: {ex}");
            }
        }

        public async void OnAppearing(CameraView cameraView)
        {
            if (Camera != null && cameraView.Cameras.Count > 0)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await cameraView.StopCameraAsync();
                    await cameraView.StartCameraAsync();
                });
            }
        }

        public async void OnDisappearing(CameraView cameraView)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
            });
        }
    }



}
