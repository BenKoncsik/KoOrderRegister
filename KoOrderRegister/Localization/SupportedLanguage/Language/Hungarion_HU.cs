using KoOrderRegister.Utility;
using KORCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Localization.SupportedLanguage.Language
{
    public class Hungarian_HU: ILanguageSettings
    {
        private string _languageName = "Hungarian_HU";
        private string _cultureName = "hu-HU";
        private string _languageDisplayName = "Magyar (Hungarian)";
        private string _languageId = "+36";

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
