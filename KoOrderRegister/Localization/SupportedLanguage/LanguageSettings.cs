using KORCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Localization.SupportedLanguage
{
    public abstract class LanguageSettings : ILanguageSettings
    {
        protected abstract string _languageName { get; }
        protected abstract string _cultureName { get; }
        protected abstract string _languageDisplayName { get; }
        protected abstract string _languageId { get; }
        protected abstract string _dateFormat { get; }
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
            DateFormat.SetLocalFormat(_dateFormat);
        }
    }

}
