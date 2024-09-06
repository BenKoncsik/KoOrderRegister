using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Localization.SupportedLanguage
{
    public interface ILanguageSettings
    {
        string DisplayName { get; }
        string GetLanguageName();
        string GetLanguageDisplayName();
        // The phone country code (e.g. +1 for US, +36 for Hun, ....)
        string GetLanguageId();
        string GetCultureName();
        Task SetRegioSpecification();


    }
}
