
using KoOrderRegister.Modules.Customer.Pages;
using KoOrderRegister.Modules.Order.Pages;
using KoOrderRegister.Modules.Settings.Pages;
using KoOrderRegister.Services;
using System.ComponentModel;

namespace KoOrderRegister
{
    public partial class AppShell : Shell
    {
       private static string _appversion = $"Ko Order-Register";
        public static Label _AppVersionLabel { get; set; } = new Label();
       public static string AppVersion 
        {
            get => _appversion;
            set
            {
                if (!_appversion.Equals(value))
                {
                    if(_AppVersionLabel != null)
                    {
                        _AppVersionLabel.Text = $"KOR V{value}";
                    }
                        
                }
            }
        }
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(OrderListPage), typeof(OrderListPage));
            Routing.RegisterRoute(nameof(CustomerListPage), typeof(CustomerListPage));
            Routing.RegisterRoute(nameof(PersonDetailsPage), typeof(PersonDetailsPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            _AppVersionLabel = AppVersionLabel;            
        }

    }
}
