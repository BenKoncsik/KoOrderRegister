
using KoOrderRegister.Modules.Export.Pdf.ViewModel;

namespace KoOrderRegister.Modules.Export.Pdf.Pages;

public partial class PdfExportersPage: ContentPage
{
	private ExportersViewModel _viewModel;
    public PdfExportersPage(ExportersViewModel exportersViewModel)
	{
        _viewModel = exportersViewModel;
        BindingContext = _viewModel;
        InitializeComponent();
    }
}