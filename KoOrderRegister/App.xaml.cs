using KoOrderRegister.Localization.SupportedLanguage;
using System.Globalization;

namespace KoOrderRegister
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        public static void RestartApp()
        {
            Application.Current.MainPage = new AppShell();
        }

        public static void LoadAppLanguage()
        {
            /*ILanguageSettings languageSettings = LanguageManager.GetCurrentLanguage();
            CultureInfo culture = new CultureInfo(languageSettings.GetCultureName());
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;*/
            RestartApp();
        }
    }
}
