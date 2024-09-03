using KoOrderRegister.Modules.Order.ViewModels;
using KoOrderRegister.Utility;

namespace KoOrderRegister.Modules.Order.Pages;

public partial class OrderListPage : ContentPage
{
    public OrderListPage(OrderListViewModel viewMode)
    {
        InitializeComponent();
        BindingContext = viewMode;
    }

    

}