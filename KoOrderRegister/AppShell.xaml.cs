using KoOrderRegister.Modules.Order.List.Pages;

namespace KoOrderRegister
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(OrderListPage), typeof(OrderListPage));
        }
    }
}
