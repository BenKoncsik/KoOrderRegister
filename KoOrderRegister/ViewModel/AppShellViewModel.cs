using KoOrderRegister.Localization;
using KoOrderRegister.Services;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.ViewModel
{
    public class AppShellViewModel: BaseViewModel
    {
        #region DI

        #endregion
        #region binding varribles
        private string _notificationString = AppRes.Notification;
        public string NotificationString
        {
            get => _notificationString;
            set
            {
                if (!_notificationString.Equals(value) && !string.IsNullOrEmpty(value))
                {
                    _notificationString = value;
                    Debug.WriteLine($"NotificationString updated to: {_notificationString}");
                    OnPropertyChanged(nameof(NotificationString));
                    if (Application.Current?.MainPage is Shell shell)
                    {
                        (Application.Current.MainPage as Shell).BindingContext = null;
                        (Application.Current.MainPage as Shell).BindingContext = this;
                    }
                    
                }
            }
        }

        private string _notificationIcon = Application.Current.PlatformAppTheme.Equals(AppTheme.Dark) ? "notifications_dark.png" : "notifications_light.png";
        public string NotificationIcon
        {
            get => _notificationIcon;
            set
            {
                if (!value.Equals(_notificationIcon))
                {
                    _notificationIcon = value;
                    Debug.WriteLine($"Notification icon updated to: {_notificationIcon}");
                    OnPropertyChanged(nameof(NotificationIcon));
                    if (Application.Current?.MainPage is Shell shell)
                    {
                        (Application.Current.MainPage as Shell).BindingContext = null;
                        (Application.Current.MainPage as Shell).BindingContext = this;
                    }
                }
            }
        }
        #endregion
        public AppShellViewModel() : base()
        {
            int count = LocalNotificationService.GetNotificationCount(); 
            if(count > 0)
            {
                NotificationString = $"{AppRes.Notification} ({count})";
            }
            LocalNotificationService.NotificationChangedStaic += Current_NotificationActionReceived;
            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
        }

        private void Current_RequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            NotificationIcon = Application.Current.PlatformAppTheme.Equals(AppTheme.Dark) ? "notifications_dark.png" : "notifications_light.png";
        }

        private void Current_NotificationActionReceived()
        {
            int count = LocalNotificationService.GetNotificationCount();
            if(count <= 0)
            {
                NotificationString = $"{AppRes.Notification}";
                NotificationIcon = Application.Current.PlatformAppTheme.Equals(AppTheme.Dark) ? "notifications_dark.png" : "notifications_light.png";
            }
            else
            {
                NotificationString = $"{AppRes.Notification} ({count})";
                NotificationIcon = Application.Current.PlatformAppTheme.Equals(AppTheme.Dark) ? "notifications_active_dark.png" : "notifications_active_light.png";
            }
        }

        
        

    }
}
