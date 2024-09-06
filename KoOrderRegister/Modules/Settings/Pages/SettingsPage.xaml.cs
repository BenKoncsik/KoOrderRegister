using KoOrderRegister.Modules.Settings.ViewModels;

namespace KoOrderRegister.Modules.Settings.Pages;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel _viewModel;
    public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}