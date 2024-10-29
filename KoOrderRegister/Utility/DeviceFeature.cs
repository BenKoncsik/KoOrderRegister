using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Utility
{
    public static class DeviceFeature
    {
        public static void VibrationOn(int millisecound = 500)
        {
            IVibration vibration = Vibration.Default;
            if (vibration.IsSupported)
            {
                try
                {
                    vibration.Vibrate(millisecound);
                }
                catch (FeatureNotSupportedException ex)
                {
                    Debug.WriteLine($"Not supported vibration: {ex}");
                }
            }
        }
    }
}
