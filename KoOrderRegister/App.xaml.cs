using KoOrderRegister.Localization;
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
            try
            {
                InitializeComponent();
                _serviceProvider = serviceProvider;
                MainPage = new AppShell(_serviceProvider);
            }
            catch(Exception ex)
            {
                ShowErrorMessage(ex);
            }
        }

      
        public static void RestartApp()
        {
            if(_serviceProvider == null)
            {
                return;
            }
            Application.Current.MainPage = new AppShell(_serviceProvider);
        }
        private static void ShowErrorMessage(Exception ex)
        {
#if WINDOWS
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logFilePath = Path.Combine(desktopPath, "log.txt");
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
            Application.Current.MainPage.DisplayAlert(AppRes.Failed, ex.Message, "OK");
#endif
        }

    }
}
