using KoOrderRegister.Modules.Export.ViewModel;

namespace KoOrderRegister.Modules.Export.Pages;

public partial class ExportersPage: ContentPage
{
	private ExportersViewModel _viewModel;
    public ExportersPage(ExportersViewModel exportersViewModel)
	{
        _viewModel = exportersViewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }
}