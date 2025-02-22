using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Localization.SupportedLanguage.Language
{
    public class Russian_RU : LanguageSettings
    {
        protected override string _languageName => "Russian_RU";

        protected override string _cultureName => "ru-RU";

        protected override string _languageDisplayName => "Русский (Russian)";

        protected override string _languageId => "+7";

        protected override string _dateFormat => @"dd/MM/yyyy hh:mm tt";
    }
}
