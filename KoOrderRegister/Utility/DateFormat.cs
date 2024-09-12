using KoOrderRegister.Localization.SupportedLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Utility
{
    public static class DateFormat
    {
        public static string DateTimeFormat { get; private set; } = @"yyyy-MM-dd hh\\:mm";

        public static void SetLocalFormat(string format)
        {
            DateTimeFormat = format;
        }
    }
}
