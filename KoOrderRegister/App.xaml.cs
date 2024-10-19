using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Services;
using System.Globalization;

namespace KoOrderRegister
{
    public partial class App : Application
    {
        private readonly AppShell _appShell;
        private static IServiceProvider _serviceProvider;
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            MainPage = new AppShell(_serviceProvider);
  
        }

      
        public static void RestartApp()
        {
            if(_serviceProvider == null)
            {
                return;
            }
            Application.Current.MainPage = new AppShell(_serviceProvider);
        }

        
    }
}
