using KoOrderRegister.Utility;
using KORCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Localization.SupportedLanguage.Language
{
    public class German_DEU : ILanguageSettings
    {
        private string _languageName = "German_DE";
        private string _cultureName = "de-DE";
        private string _languageDisplayName = "Deutsch (German)";
        private string _languageId = "+49";

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
            DateFormat.SetLocalFormat(@"yyyy.MM.dd HH:mm");
        }
    }
}
