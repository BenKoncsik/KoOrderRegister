using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.EntryCheckers
{
    public abstract class LocalizedBehavior<T> : Behavior<T> where T : BindableObject
    {
        protected override void OnAttachedTo(T bindable)
        {
            base.OnAttachedTo(bindable);
            SetLocalizedBehavior(bindable);
        }

        protected abstract void SetLocalizedBehavior(T bindable);
    }
}
