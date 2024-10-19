using Plugin.LocalNotification.AndroidOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Windows.Notification.Utils
{
    public class WindowsOptions
    {
        public bool Ongoing { get; set; }
        public Color Color { get; set; } = new();
        public string IconName { get; set; }
        public bool LaunchAppWhenTapped { get; set; } = true;
        public int? LedColor { get; set; }
        public WindowsPriority Priority { get; set; } = WindowsPriority.Default;
        public WindowsProgressBar ProgressBar { get; set; } = new WindowsProgressBar();
        public TimeSpan? TimeoutAfter { get; set; }
        public long[] VibrationPattern { get; set; } = [];
        public DateTime? When { get; set; }
    }
}
