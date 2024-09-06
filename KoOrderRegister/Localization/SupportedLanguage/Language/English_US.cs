using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Localization.SupportedLanguage
{
    public class English_US : ILanguageSettings
    {
        private string _languageName = "English_US";
        private string _cultureName = "en-US";
        private string _languageDisplayName = "English (United States)";
        private string _languageId = "+1";

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
            
        }
    }
}
