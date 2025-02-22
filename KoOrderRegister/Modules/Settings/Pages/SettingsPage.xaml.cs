using KoOrderRegister.Modules.Settings.ViewModels;
using KoOrderRegister.Utility;
using KORCore.Utility;

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

    protected override void OnAppearing()
    {
        using (new LowPriorityTaskManager())
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}