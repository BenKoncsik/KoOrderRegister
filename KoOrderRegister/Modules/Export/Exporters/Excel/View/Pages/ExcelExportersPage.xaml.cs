using KoOrderRegister.Modules.Export.Excel.ViewModel;

namespace KoOrderRegister.Modules.Export.Excel.Pages;

public partial class ExcelExportersPage: ContentPage
{
	private ExportersViewModel _viewModel;
    public ExcelExportersPage(ExportersViewModel exportersViewModel)
	{
        _viewModel = exportersViewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }
}