using KoOrderRegister.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Localization.SupportedLanguage
{
    public static class LanguageManager
    {
        public static List<ILanguageSettings> LanguageSettingsInstances = new List<ILanguageSettings>();

        static LanguageManager()
        {
            LoadLanguageSettings();
        }

        private static void LoadLanguageSettings()
        {
            var languageSettingsTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ILanguageSettings).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            foreach (var type in languageSettingsTypes)
            {
                ILanguageSettings instance = (ILanguageSettings)Activator.CreateInstance(type);
                LanguageSettingsInstances.Add(instance);
            }
        }

        public static ILanguageSettings GetLanguageSettings(string languageId)
        {
            return LanguageSettingsInstances.FirstOrDefault(x => x.GetLanguageId() == languageId) ?? new English_US();
        }

        public static ILanguageSettings GetCurrentLanguage()
        {
            string savedLanguage = Preferences.Get("selected_language", "+1");
            return GetLanguageSettings(savedLanguage);
        }

        public static void SetLanguage(ILanguageSettings languageSettings)
        {
            Preferences.Set("selected_language", languageSettings.GetLanguageId());
            CultureInfo culture = new CultureInfo(languageSettings.GetCultureName());
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            languageSettings.SetRegioSpecification();
            App.RestartApp();
        }
    }
}
