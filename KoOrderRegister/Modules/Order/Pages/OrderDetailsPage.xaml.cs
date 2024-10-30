using KoOrderRegister.Utility;
using System.Collections.ObjectModel;
using KoOrderRegister.Modules.Order.ViewModels;
using KORCore.Modules.Database.Models;
using KORCore.Utility;

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
            _viewModel.UpdateFiles();
        }
        else
        {
            if (_viewModel.Files != null)
            {
                _viewModel.Files.Clear();
            }
        }
        
    }

    protected override void OnAppearing()
    {
        using (new LowPriorityTaskManager())
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
            _viewModel.Update();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if(_viewModel.Files != null)
        {
            _viewModel.Files.Clear();
        }
        _viewModel.IsEdit = false;
        _viewModel.Order = new OrderModel();
        _viewModel.SelectedStartTime = DateTime.Now.TimeOfDay;
        _viewModel.SelectedEndTime = DateTime.Now.TimeOfDay;
        _viewModel.SelectedStartDate = DateTime.Now.Date;
        _viewModel.SelectedEndDate = DateTime.Now.Date;
    }

}	