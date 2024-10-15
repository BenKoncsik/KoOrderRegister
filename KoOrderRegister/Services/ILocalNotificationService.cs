using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.LocalNotification.EventArgs;

namespace KoOrderRegister.Services
{
    public interface ILocalNotificationService
    {
        int SendNotification(string title, string message, NotificationCategoryType categoryType = NotificationCategoryType.None, AndroidOptions androidOptions = null, iOSOptions iosOption = null);
        void UpdateNotification(int id, string title, string message, NotificationCategoryType categoryType = NotificationCategoryType.None, AndroidOptions androidOptions = null, iOSOptions iosOption = null);
        void DeleteNotification(int id);
        event Action<NotificationActionArgs> NotificationReceived;
    }

    public class NotificationActionArgs : EventArgs
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
