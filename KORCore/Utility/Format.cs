using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Utility
{
    public static class Format
    {
        public static string FormatSize(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B"; 
            else if (bytes < 1024 * 1024)
                return $"{bytes / 1024.0:F2} KB"; 
            else if (bytes < 1024 * 1024 * 1024)
                return $"{bytes / (1024.0 * 1024.0):F2} MB"; 
            else if (bytes < 1024L * 1024 * 1024 * 1024)
                return $"{bytes / (1024.0 * 1024 * 1024.0):F2} GB"; 
            else
                return $"{bytes / (1024.0 * 1024 * 1024 * 1024.0):F2} TB"; 
        }

        public static string ToStringSize(this long? bytesSize)
        {
            if(bytesSize.HasValue)
            {
                return FormatSize(bytesSize.Value);
            }
            else
            {
                return "0 B";
            } 
        }

        public static string ToStringSize(this long bytesSize)
        {
            bytesSize = bytesSize == null? 0 : bytesSize;   
            return FormatSize(bytesSize);
        }

        
    }
}
