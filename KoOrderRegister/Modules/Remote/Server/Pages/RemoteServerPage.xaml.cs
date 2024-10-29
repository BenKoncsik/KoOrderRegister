using Camera.MAUI;
using KoOrderRegister.Modules.Remote.Server.Service;
using KoOrderRegister.Modules.Remote.ViewModel;

namespace KoOrderRegister.Modules.Remote.Server.Pages;

public partial class RemoteServerPage : ContentPage
{
	private readonly RemoteServerViewModel _viewModel;
    public RemoteServerPage(RemoteServerViewModel remoteServerViewModel)
	{
        _viewModel = remoteServerViewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        _viewModel.OnAppearing();
        base.OnAppearing();
    }
}