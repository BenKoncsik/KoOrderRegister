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



namespace KoOrderRegister.Services
{
    public interface ILocalNotificationService
    {
        event Action<NotificationActionArgs> NotificationReceived;
        event Action<NotificationChangedArgs> NotificationChanged;
        event Action<NotificationClearedArgs> NotificationCleared;
        ConcurrentDictionary<int, NotificationChangedArgs> Notifications { get; }
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

    public class NotificationChangedArgs
    {
        public NotificationRequest NotificationRequest { get; set; }
        public AndroidOptions AndroidOption { get; set; }
        public iOSOptions IosOption { get; set; }
        public WindowsOptions WindowsOption { get; set; }
        public bool IsProgressBar { get; set; }
    }

    public struct NotificationClearedArgs
    {
        public int Id { get; set; }
    }

}
