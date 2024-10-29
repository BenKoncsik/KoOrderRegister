using KoOrderRegister.Modules.Order.ViewModels;
using KoOrderRegister.Utility;
using KORCore.Utility;
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
        using (new LowPriorityTaskManager())
        {
            base.OnAppearing();
            _viewModel.UpdateOrders();
        }
    }

    protected void OnTextChanged(object sender, EventArgs e)
    {
        SearchBar searchBar = (SearchBar)sender;
        _viewModel.Search(searchBar.Text);
    }

    private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
    {
        var items = _viewModel.Orders;

        if (items != null && items.Count > 0 && e.Item == items[items.Count - 1])
        {
         //   _viewModel.LoadMoreItems();
        }
    }
}