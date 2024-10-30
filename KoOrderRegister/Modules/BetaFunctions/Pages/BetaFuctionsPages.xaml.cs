using KoOrderRegister.Modules.BetaFunctions.ViewModels;
using KoOrderRegister.Utility;
using KORCore.Utility;
using System.Diagnostics;

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

    protected override void OnAppearing()
    {
        using (new LowPriorityTaskManager())
        {
            Debug.WriteLine($"Navigate to: BetaFunctionsPages");
            _viewModel.OnAppearing();
            base.OnAppearing();
        }
    }
}