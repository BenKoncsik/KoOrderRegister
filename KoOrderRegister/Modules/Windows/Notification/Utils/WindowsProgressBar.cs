using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Windows.Notification.Utils
{
    public class WindowsProgressBar
    {
            /// <summary>
            /// Set whether this progress bar is in indeterminate mode
            /// </summary>
            public bool IsIndeterminate { get; set; }

            /// <summary>
            /// Set Upper limit of this progress bar's range
            /// </summary>
            public int Max { get; set; }

            /// <summary>
            /// Set progress bar's current level of progress
            /// </summary>
            public int Progress { get; set; }
        }
}
