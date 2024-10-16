using KoOrderRegister.Modules.Windows.Notification.ViewModel;

namespace KoOrderRegister.Modules.Windows.Notification.Pages;

public partial class NotificationPages : ContentPage
{
	private readonly NotificationViewModel _notificationViewModel;
    public NotificationPages(NotificationViewModel notificationViewModel)
	{
        _notificationViewModel = notificationViewModel;
        BindingContext = _notificationViewModel;
        InitializeComponent();
    }


    private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
    {
       
    }
    protected override void OnAppearing()
    {
        _notificationViewModel.Appering();
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
       _notificationViewModel.Disappearing();
    }
}