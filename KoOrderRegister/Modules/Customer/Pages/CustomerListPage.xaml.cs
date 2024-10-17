using KoOrderRegister.Modules.Customer.ViewModels;
using KoOrderRegister.Utility;

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
    private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
    {
        var items = _viewModel.Customers;

        if (items != null && e.Item == items[items.Count - 1])
        {
          //  _viewModel.LoadMoreItems();
        }
    }
    protected override void OnAppearing()
    {
        using (new LowPriorityTaskManager())
        {
            base.OnAppearing();
            _viewModel.Update();
        }
    }

    protected void OnTextChanged(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        _viewModel.Search(searchBar.Text);
    }
}