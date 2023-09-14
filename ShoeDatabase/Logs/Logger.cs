using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Logs
{
    public static class Logger
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorLog_" + new DateTime().ToString() + ".txt");

        public static void LogException(Exception ex)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine($"Date: {DateTime.Now}");
                    sw.WriteLine(ex.ToString());
                    sw.WriteLine("------------------------------------------------------------");
                }
            }
            catch
            {

            }
        }
    }
}
