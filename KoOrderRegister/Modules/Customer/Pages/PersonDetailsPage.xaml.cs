using KoOrderRegister.Modules.Customer.ViewModels;
using KoOrderRegister.Utility;
using KORCore.Modules.Database.Models;
using KORCore.Utility;

namespace KoOrderRegister.Modules.Customer.Pages;

public partial class PersonDetailsPage : ContentPage
{
    private readonly PersonDetailsViewModel _viewModel;
    public PersonDetailsPage(PersonDetailsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    public void EditCustomer(CustomerModel customer)
    {
        _viewModel.IsEdit = true;
        _viewModel.Customer = customer;
    }

    protected override void OnAppearing()
    {
        using (new LowPriorityTaskManager())
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Customer = new CustomerModel();
        _viewModel.IsEdit = false;
    }
}