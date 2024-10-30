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
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                LogError(args.ExceptionObject as Exception);
            };
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
        private void LogError(Exception ex)
        {
#if WINDOWS
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string logFileLocalPath = Path.Combine(localPath, "error.txt");
            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string romingLocalPath = Path.Combine(roamingPath, "error.txt");

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var logFilePath = Path.Combine(desktopPath, "error.txt");

            var message = $"Timestamp: {DateTime.Now}\nException: {ex}\n";

            File.AppendAllText(logFilePath, message);
            File.AppendAllText(logFileLocalPath, message);
            File.AppendAllText(romingLocalPath, message);
            MainPage.Dispatcher.Dispatch(() =>
            {
                MainPage.DisplayAlert("Hiba történt", $"Egy hiba történt: {ex.Message}\nAz esemény naplózva lett.", "OK");
            });
#endif
        }

    }
}
