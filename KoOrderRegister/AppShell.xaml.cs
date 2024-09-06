
using KoOrderRegister.Modules.Customer.Pages;
using KoOrderRegister.Modules.Order.Pages;
using KoOrderRegister.Modules.Settings.Pages;

namespace KoOrderRegister
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(OrderListPage), typeof(OrderListPage));
            Routing.RegisterRoute(nameof(CustomerListPage), typeof(CustomerListPage));
            Routing.RegisterRoute(nameof(PersonDetailsPage), typeof(PersonDetailsPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}
