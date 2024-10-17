using KoOrderRegister.Localization.SupportedLanguage;
using KoOrderRegister.Services;
using System.Globalization;

namespace KoOrderRegister
{
    public partial class App : Application
    {
        private readonly AppShell _appShell;
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
  
        }

      
        public static void RestartApp()
        {
            Application.Current.MainPage = new AppShell();
        }

        
    }
}
