using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Services
{
    public class LocalNotificationWindows : ILocalNotificationService
    {
        public event Action<NotificationActionArgs> NotificationReceived;
        public void DeleteNotification(int id)
        {
            
        }

        public int SendNotification(string title, string message, NotificationCategoryType categoryType = NotificationCategoryType.None, AndroidOptions androidOptions = null, iOSOptions iosOption = null)
        {
            return new Random().Next(0, int.MaxValue);
        }

        public void UpdateNotification(int id, string title, string message, NotificationCategoryType categoryType = NotificationCategoryType.None, AndroidOptions androidOptions = null, iOSOptions iosOption = null)
        {
         
        }
    }
}
