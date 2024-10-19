using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.LocalNotification.EventArgs;
using KoOrderRegister.Modules.Windows.Notification.Utils;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.ComponentModel;
using DocumentFormat.OpenXml.Presentation;



namespace KoOrderRegister.Services
{
    public interface ILocalNotificationService
    {
        event Action<NotificationActionArgs> NotificationReceived;
        event Action<NotificationChangedArgs> NotificationChanged;
        event Action<NotificationClearedArgs> NotificationCleared;
        int SendNotification(string title, string message, NotificationCategoryType categoryType = NotificationCategoryType.None, AndroidOptions androidOptions = null, iOSOptions iosOption = null, WindowsOptions windowsOptions = null);
        void UpdateNotification(int id, string title, string message, NotificationCategoryType categoryType = NotificationCategoryType.None, AndroidOptions androidOptions = null, iOSOptions iosOption = null, WindowsOptions windowsOptions = null);
        void DeleteNotification(int id);
        void ClickNotification(NotificationActionArgs notificationArgs);
        
    }

    public class NotificationActionArgs : EventArgs
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class NotificationChangedArgs : INotifyPropertyChanged
    {
        private NotificationRequest _notificationRequest;
        private AndroidOptions _androidOption;
        private iOSOptions _iosOption;
        private WindowsOptions _windowsOption;
        private bool _isProgressBar;

        public NotificationRequest NotificationRequest
        {
            get => _notificationRequest;
            set
            {
                if (_notificationRequest != value)
                {
                    _notificationRequest = value;
                    OnPropertyChanged(nameof(NotificationRequest));
                }
            }
        }

        public AndroidOptions AndroidOption
        {
            get => _androidOption;
            set
            {
                if (_androidOption != value)
                {
                    _androidOption = value;
                    OnPropertyChanged(nameof(AndroidOption));
                }
            }
        }

        public iOSOptions IosOption
        {
            get => _iosOption;
            set
            {
                if (_iosOption != value)
                {
                    _iosOption = value;
                    OnPropertyChanged(nameof(IosOption));
                }
            }
        }

        public WindowsOptions WindowsOption
        {
            get
            {
                if(_windowsOption == null)
                {
                    _windowsOption = new WindowsOptions();
                }
                return _windowsOption;
            }
            set
            {
                if (_windowsOption != value)
                {
                    _windowsOption = value;
                    OnPropertyChanged(nameof(WindowsOption));
                }
            }
        }

        public bool IsProgressBar
        {
            get => _isProgressBar;
            set
            {
                if (_isProgressBar != value)
                {
                    _isProgressBar = value;
                    OnPropertyChanged(nameof(IsProgressBar));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public struct NotificationClearedArgs
    {
        public int Id { get; set; }
    }

}
