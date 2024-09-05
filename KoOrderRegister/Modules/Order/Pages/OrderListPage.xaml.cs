using KoOrderRegister.Modules.Order.ViewModels;
using KoOrderRegister.Utility;
using System.Collections.Generic;

namespace KoOrderRegister.Modules.Order.Pages;

public partial class OrderListPage : ContentPage
{
    private readonly OrderListViewModel _viewMode;
    public OrderListPage(OrderListViewModel viewMode)
    {
        InitializeComponent();
        _viewMode = viewMode;
        BindingContext = _viewMode;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewMode.UpdateOrders();
    }

}