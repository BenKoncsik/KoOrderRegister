using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Order.List.ViewModels;

namespace KoOrderRegister.Modules.Order.Pages;

public partial class OrderDetailsPage : ContentPage
{
	private readonly OrderDetailViewModel _viewModel;
    public OrderDetailsPage(OrderDetailViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public void EditOrder(OrderModel order)
    {
        _viewModel.IsEdit = true;
        _viewModel.EditOrder(order);
        if(order.Files != null)
        {
            if (_viewModel.Files != null)
            {
                _viewModel.Files.Clear();
            }
            foreach (var file in order.Files)
            {
                _viewModel.Files.Add(file);
            }
        }
        
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Update();
    }

}	