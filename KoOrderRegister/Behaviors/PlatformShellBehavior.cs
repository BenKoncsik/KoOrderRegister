using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Behaviors
{
    public class PlatformShellBehavior : Behavior<Shell>
    {
        public string Platforms { get; set; }
        public bool IsSupported => BehaviorLogic.IsSupportedPlatform(Platforms);

    }


    public class PlatformBehavior : Behavior<VisualElement>
    {
        public string Platforms { get; set; }

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);

            if (!BehaviorLogic.IsSupportedPlatform(Platforms))
            {
                bindable.IsVisible = false;
            }
        }

        private void SetVisibility(VisualElement bindable)
        {
            bindable.IsVisible = BehaviorLogic.IsSupportedPlatform(Platforms);
        }
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            
        }
    }
    internal static class BehaviorLogic
    {
        public static bool IsSupportedPlatform(string platforms)
        {
            if (string.IsNullOrWhiteSpace(platforms))
            {
                return true;
            }

            var supportedPlatforms = platforms.Split(',').Select(p => p.Trim().ToLowerInvariant());
#if ANDROID
            return supportedPlatforms.Contains("android");
#elif WINDOWS
            return supportedPlatforms.Contains("windows");
#elif IOS
            return supportedPlatforms.Contains("ios");
#elif MACCATALYST
            return supportedPlatforms.Contains("maccatalyst");
#else
            return false;
#endif
        }
    }
}
