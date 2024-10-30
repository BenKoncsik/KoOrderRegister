using KoOrderRegister.Modules.Windows.Notification.ViewModel;

namespace KoOrderRegister.Modules.Windows.Notification.Pages;

public partial class NotificationPages : ContentPage
{
	private readonly NotificationViewModel _viewModel;
    public NotificationPages(NotificationViewModel notificationViewModel)
	{
        _viewModel = notificationViewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }


    private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
    {
       
    }

    public void LoadData()
    {
      _viewModel.OnAppearing();
    }
    protected override void OnAppearing()
    {
        _viewModel.OnAppearing();
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Disappearing();
    }
}