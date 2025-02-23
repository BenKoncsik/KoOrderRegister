using KoOrderRegister.Modules.About.ViewModel;

namespace KoOrderRegister.Modules.About.Pages;

public partial class AboutPage : ContentPage
{
	private readonly AboutViewModel _viewModel;
    public AboutPage(AboutViewModel aboutViewModel)
	{
		this._viewModel = aboutViewModel;
        BindingContext = _viewModel;
        InitializeComponent();
	}
}