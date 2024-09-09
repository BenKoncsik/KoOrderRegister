using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KoOrderRegister.EntryCheckers.Phone
{
    public class PhoneNumberValidationBehavior : LocalizedBehavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEntryTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= OnEntryTextChanged;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (entry == null)
                return;

            entry.Text = FormatPhoneNumber(e.NewTextValue);
        }

        private string FormatPhoneNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            var digits = Regex.Replace(input, @"[^\d\+]", "");

            // Check if the number starts with '+'
            var plusPrefix = digits.StartsWith("+") ? "+" : "";
            if (plusPrefix == "+")
            {
                digits = digits.Substring(1);
            }

            if (digits.Length > 15)
            {
                digits = digits.Substring(0, 15);
            }

            digits = plusPrefix + digits;

            return digits;
        }

        protected override void SetLocalizedBehavior(Entry bindable)
        {
            
        }
    }
}
