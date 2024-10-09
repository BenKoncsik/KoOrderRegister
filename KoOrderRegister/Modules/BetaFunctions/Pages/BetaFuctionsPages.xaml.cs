using KoOrderRegister.Modules.BetaFunctions.ViewModels;

namespace KoOrderRegister.Modules.BetaFunctions.Pages;

public partial class BetaFunctionsPages : ContentPage
{
	private readonly BetaFunctionsViewModel _viewModel;
    public BetaFunctionsPages(BetaFunctionsViewModel viewModel)
	{
        _viewModel = viewModel;
        BindingContext = _viewModel;
        InitializeComponent();
	}
}