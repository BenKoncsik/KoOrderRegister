
using KoOrderRegister.Modules.Export.Exporters.Pdf.View.ViewModel;
using KoOrderRegister.Utility;

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

    protected override void OnAppearing()
    {
        using (new LowPriorityTaskManager())
        {
            base.OnAppearing();
        }
    }
}