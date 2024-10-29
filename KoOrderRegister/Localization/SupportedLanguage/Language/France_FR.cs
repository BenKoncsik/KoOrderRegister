using KoOrderRegister.Utility;
using KORCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Localization.SupportedLanguage.Language
{
    public class France_FR : ILanguageSettings
    {
        private string _languageName = "France_FR";
        private string _cultureName = "fr-FR";
        private string _languageDisplayName = "France (France) [AI Translated]";
        private string _languageId = "+33";

        public string DisplayName { get => GetLanguageDisplayName(); }

        public string GetLanguageDisplayName()
        {
            return _languageDisplayName;
        }

        public string GetLanguageId()
        {
            return _languageId;
        }

        public string GetLanguageName()
        {
            return _languageDisplayName;
        }

        public string GetCultureName()
        {
            return _cultureName;
        }

        public async Task SetRegioSpecification()
        {
            DateFormat.SetLocalFormat(@"dd-MM-yyyy HH:mm");
        }
    }
}
