using KoOrderRegister.Modules.Order.ViewModels;
using KoOrderRegister.Utility;
using System.Collections.Generic;

namespace KoOrderRegister.Modules.Order.Pages;

public partial class OrderListPage : ContentPage
{
    private readonly OrderListViewModel _viewModel;
    public OrderListPage(OrderListViewModel viewMode)
    {
        InitializeComponent();
        _viewModel = viewMode;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.UpdateOrders();
    }

    protected void OnTextChanged(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        _viewModel.Search(searchBar.Text);
    }
}