using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Windows.Notification.Utils
{
    public class WindowsProgressBar: INotifyPropertyChanged
    {
        /// <summary>
        /// Set whether this progress bar is in indeterminate mode
        /// </summary>
        public bool IsIndeterminate { get; set; } = false;

        /// <summary>
        /// Set Upper limit of this progress bar's range
        /// </summary>
        public int Max { get; set; } = 0;

        private double progress = 0;

        /// <summary>
        /// Set progress bar's current level of progress
        /// </summary>
        public double Progress
        {
            get => progress;
            set
            {
                if (value >= 0 && value <= Max)
                {
                    progress = value / 100;
                    Debug.WriteLine($"Windows progress bar: {progress}");
                    OnPropertyChanged(nameof(Progress));
                }
                else
                {
                    progress = Max / 100;
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (TargetInvocationException ex)
            {
                Debug.WriteLine($"Inner Exception: {ex.InnerException}");
            }

        }
    }
}
