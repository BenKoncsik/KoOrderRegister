using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Localization.SupportedLanguage
{
    public interface ILanguageSettings
    {
        Task<string> GetLanguageName();
        Task<string> GetLanguageDisplayName();
        Task<long> GetLanguageId();
        Task SetRegioSpecification();


    }
}
