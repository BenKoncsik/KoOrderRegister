using Camera.MAUI;
using KoOrderRegister.Modules.Remote.Client.ViewModel;

namespace KoOrderRegister.Modules.Remote.Client.Pages;

public partial class ClientConnectionPage : ContentPage
{
	private readonly ClientConnectionViewModel _viewModel;
    public ClientConnectionPage(ClientConnectionViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing(cameraView);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnDisappearing(cameraView);
    }
}