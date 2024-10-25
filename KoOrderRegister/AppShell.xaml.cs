using KoOrderRegister.Localization;
using KoOrderRegister.Modules.BetaFunctions.Pages;
using KoOrderRegister.Modules.Customer.Pages;
using KoOrderRegister.Modules.Order.Pages;
using KoOrderRegister.Modules.Settings.Pages;
using KoOrderRegister.Modules.Windows.Notification.Pages;
using KoOrderRegister.Services;
using KoOrderRegister.Utility;
using KoOrderRegister.ViewModel;
using Microsoft.VisualBasic;
using Plugin.LocalNotification;
using System.ComponentModel;
using System.Diagnostics;

namespace KoOrderRegister
{
    public partial class AppShell : Shell
    {
       private static string _appversion = $"Ko Order-Register";
       public static Label _AppVersionLabel { get; set; } = new Label();
#if DEBUG || DEVBUILD
        public static readonly bool IsDevBuild = true;
#else
        public static readonly bool IsDevBuild = false;
#endif
#if WINDOWS
        public static readonly bool IsWindows = true;
#else
        public static readonly bool IsWindows = false;
#endif
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
        private readonly AppShellViewModel _appShellViewModel;
        private readonly IServiceProvider _serviceProvider;
        public AppShell(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _appShellViewModel = new AppShellViewModel();
            BindingContext = _appShellViewModel;
            #region Stabil fuctions
            Routing.RegisterRoute(nameof(OrderListPage), typeof(OrderListPage));
            Routing.RegisterRoute(nameof(CustomerListPage), typeof(CustomerListPage));
            Routing.RegisterRoute(nameof(PersonDetailsPage), typeof(PersonDetailsPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

            #endregion
            #region Beta fuctions
            Routing.RegisterRoute(nameof(BetaFunctionsPages), typeof(BetaFunctionsPages));
            Routing.RegisterRoute(nameof(Modules.Export.Excel.Pages.ExcelExportersPage), typeof(Modules.Export.Excel.Pages.ExcelExportersPage));
            Routing.RegisterRoute(nameof(Modules.Export.Pdf.Pages.PdfExportersPage), typeof(Modules.Export.Pdf.Pages.PdfExportersPage));
            Routing.RegisterRoute(nameof(Modules.Export.Html.Pages.HtmlExportersPage), typeof(Modules.Export.Html.Pages.HtmlExportersPage));
            #endregion
            #region Windows fuctions
            Routing.RegisterRoute(nameof(NotificationPages), typeof(NotificationPages));
#if WINDOWS
            var notificationPage = serviceProvider.GetService<NotificationPages>();
            notificationPage.LoadData();
#endif
            #endregion
            #region Android fuctions
            #endregion
            #region IOS fuctions
            #endregion
            #region MacOs fuctions
            #endregion

            _AppVersionLabel = AppVersionLabel;

            
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            using (new LowPriorityTaskManager())
            {
                _appShellViewModel.IsRefreshing = true;
                Debug.WriteLine($"Navigated: {args.Source}");
                base.OnNavigated(args);
#if WINDOWS
                var shellItem = Shell.Current?.CurrentItem;
                string title = shellItem?.Title;

                //titleLabel.Text = $"{title} {_appShellViewModel.NotificationString}";
#endif
            }
        }
    }
}
