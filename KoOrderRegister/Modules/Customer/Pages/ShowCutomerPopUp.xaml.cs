using KoOrderRegister.Modules.Customer.ViewModels;
using KoOrderRegister.Modules.Database.Models;
using Mopups.Pages;

namespace KoOrderRegister.Modules.Customer.Pages;

public partial class ShowCustomerPopUp : PopupPage
{
	private readonly PersonDetailPopUp _viewModel;
    public ShowCustomerPopUp(PersonDetailPopUp viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public void EditCustomer(CustomerModel customer)
    {
        _viewModel.Customer = customer;
    }
}