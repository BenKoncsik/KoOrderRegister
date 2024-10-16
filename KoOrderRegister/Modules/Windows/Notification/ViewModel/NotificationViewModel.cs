using KoOrderRegister.Services;
using KoOrderRegister.ViewModel;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KoOrderRegister.Modules.Windows.Notification.ViewModel
{
    public class NotificationViewModel : BaseViewModel
    {
        private readonly ILocalNotificationService _localNotificationService;

        #region Binding varribles
        public ObservableCollection<NotificationChangedArgs> Notifications { get; set; } = new ObservableCollection<NotificationChangedArgs>();
        #endregion
        #region Commands
        public ICommand ClickNotificationCommand => new Command<NotificationChangedArgs>(ClickNotification);
        public ICommand DeleteOrderCommand => new Command<NotificationChangedArgs>(RemoveNotification);
        #endregion

        public NotificationViewModel(ILocalNotificationService localNotificationService)
        {
            _localNotificationService = localNotificationService;
            _localNotificationService.NotificationChanged += LocalNotificationService_NotificationChanged;
            _localNotificationService.NotificationCleared += LocalNotificationService_NotificationCleared;
        }
        public void LocalNotificationService_NotificationChanged(NotificationChangedArgs notificationChangedArgs)
        {
            Device.BeginInvokeOnMainThread(() => {
             
                NotificationChangedArgs? notification = Notifications?.FirstOrDefault(n => n.NotificationRequest.NotificationId.Equals(notificationChangedArgs.NotificationRequest.NotificationId));
                if (notification != null)
                {
                        notification.WindowsOption = notificationChangedArgs.WindowsOption;
                        notification.AndroidOption = notificationChangedArgs.AndroidOption;
                        notification.IosOption = notificationChangedArgs.IosOption;
                        notification.IsProgressBar = notificationChangedArgs.IsProgressBar;
                        notification.NotificationRequest = notificationChangedArgs.NotificationRequest;
                }
                else
                {
                    Notifications.Add(notificationChangedArgs);
                }

                
            });

        }
        public void LocalNotificationService_NotificationCleared(NotificationClearedArgs notificationClearedArgs)
        {
            Device.BeginInvokeOnMainThread(() => {
                Notifications.Remove(Notifications.FirstOrDefault(x => x.NotificationRequest.NotificationId == notificationClearedArgs.Id));
            });
        }
        public void ClickNotification(NotificationChangedArgs notificationChangedArgs)
        {
            _localNotificationService.ClickNotification(new NotificationActionArgs
            {
                Id = notificationChangedArgs.NotificationRequest.NotificationId,
                Title = notificationChangedArgs.NotificationRequest.Title,
            });
        }

        public void RemoveNotification(NotificationChangedArgs notificationClearedArgs)
        {
            _localNotificationService.DeleteNotification(notificationClearedArgs.NotificationRequest.NotificationId);
        }

        internal void Appering()
        {
            _localNotificationService.NotificationChanged += LocalNotificationService_NotificationChanged;
            _localNotificationService.NotificationCleared += LocalNotificationService_NotificationCleared;
        }

        internal void Disappearing()
        { 
            _localNotificationService.NotificationChanged -= LocalNotificationService_NotificationChanged;
            _localNotificationService.NotificationCleared -= LocalNotificationService_NotificationCleared;
        }
    }
}
