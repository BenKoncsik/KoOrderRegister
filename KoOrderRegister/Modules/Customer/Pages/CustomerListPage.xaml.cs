using KoOrderRegister.Modules.Customer.ViewModels;

namespace KoOrderRegister.Modules.Customer.Pages;

public partial class CustomerListPage : ContentPage
{
    private readonly CustomerListViewModel _viewModel;
    public CustomerListPage(CustomerListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = _viewModel;
	}

    /*
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _viewModel.Update();
    }*/

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Update();
    }

    protected void OnTextChanged(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        _viewModel.Search(searchBar.Text);
    }
}