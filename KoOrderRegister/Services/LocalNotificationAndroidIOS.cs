using DocumentFormat.OpenXml.Presentation;
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
    public class LocalNotificationAndroidIOS : ILocalNotificationService
    {
        public event Action<NotificationActionArgs> NotificationReceived;
        public LocalNotificationAndroidIOS()
        {
            LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
        }

        private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            NotificationReceived?.Invoke(new NotificationActionArgs
            {
                Id = e.Request.NotificationId,
                Title = e.Request.Title,
            });
        }

        public int SendNotification(string title, string message, NotificationCategoryType categoryType = NotificationCategoryType.None, AndroidOptions androidOptions = null, iOSOptions iosOption = null)
        {
            int id = new Random().Next(0, int.MaxValue);
            var request = new NotificationRequest
            {
                NotificationId = id,
                Title = title,
                Description = message,
                CategoryType = categoryType,
                Silent = false,
                Android =
                {
                    ChannelId = "kor_general",
                    IconSmallName =
                    {
                          ResourceName = "appicon",
                    },
                }
            };

            if (androidOptions != null)
            {
                request.Android = androidOptions;
            }
            if (iosOption != null)
            {
                request.iOS = iosOption;
            }

            LocalNotificationCenter.Current.Show(request);
            return id;
        }

        public void UpdateNotification(int id, string title, string message, NotificationCategoryType categoryType = NotificationCategoryType.None, AndroidOptions androidOptions = null, iOSOptions iosOption = null)
        {
            var request = new NotificationRequest
            {
                NotificationId = id,
                Title = title,
                Description = message,
                CategoryType = categoryType,
                Silent = true,
                Android =
                {
                    ChannelId = "kor_general",
                    IconSmallName =
                    {
                          ResourceName = "appicon",
                    },
                    PendingIntentFlags = AndroidPendingIntentFlags.UpdateCurrent,
                }               
            };

            if (androidOptions != null)
            {
                request.Android = androidOptions;
            }
            if (iosOption != null)
            {
                request.iOS = iosOption;
            }


            LocalNotificationCenter.Current.Show(request);
        }

        public void DeleteNotification(int id)
        {
            LocalNotificationCenter.Current.Clear(id);
        }

       
    }
}
