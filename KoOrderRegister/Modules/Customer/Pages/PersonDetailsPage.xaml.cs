using KoOrderRegister.Modules.Customer.ViewModels;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Utility;

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
        }
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Customer = new CustomerModel();
        _viewModel.IsEdit = false;
    }
}