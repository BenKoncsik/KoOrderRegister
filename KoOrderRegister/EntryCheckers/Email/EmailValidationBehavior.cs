using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.EntryCheckers.Email
{
    public class EmailValidationBehavior : LocalizedBehavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(bindable);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Entry entry)
            {
                if (IsValidEmail(args.NewTextValue))
                {
                    entry.TextColor = Application.Current.PlatformAppTheme.Equals(AppTheme.Dark) ? Colors.White : Colors.Black;
                }
                else
                {
                    entry.TextColor = Colors.Red; 
                }
            }
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected override void SetLocalizedBehavior(Entry bindable)
        {
            
        }
    }
}
