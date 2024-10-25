using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Module.Database.Utility
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
