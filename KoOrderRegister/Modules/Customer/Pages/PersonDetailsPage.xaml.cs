using KoOrderRegister.Modules.Customer.ViewModels;
using KoOrderRegister.Modules.Database.Models;

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
}