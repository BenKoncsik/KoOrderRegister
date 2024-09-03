using KoOrderRegister.Modules.Customer.ViewModels;

namespace KoOrderRegister.Modules.Customer.Pages;

public partial class CustomerListPage : ContentPage
{
	public CustomerListPage(CustomerListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}