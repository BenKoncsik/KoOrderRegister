using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace KoOrderRegister.EntryCheckers.INS
{
    public class BaseINSBehavior : LocalizedBehavior<Entry>
    {
        protected override void SetLocalizedBehavior(Entry bindable)
        {

        }
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
            if (entry == null || e.NewTextValue == null)
            {
                return;
            }
            var cleanedText = e.NewTextValue.Replace(" ", "");

            if (!string.IsNullOrWhiteSpace(cleanedText))
            {
                var formattedText = Regex.Replace(cleanedText, @"(\d{3})(?=\d)", "$1 ");

                
                int cursorPosition = entry.CursorPosition;
                cursorPosition = Math.Min(cursorPosition, formattedText.Length);

                if (e.OldTextValue != null && e.OldTextValue.Length > cleanedText.Length)
                {

                    int oldSpaceCount = e.OldTextValue.Substring(0, Math.Min(cursorPosition, e.OldTextValue.Length)).Count(c => c == ' ');
                    int newSpaceCount = formattedText.Substring(0, cursorPosition).Count(c => c == ' ');

                    cursorPosition -= (oldSpaceCount - newSpaceCount);
                }

                if (entry.Text != formattedText.TrimEnd())
                {
                    entry.Text = formattedText.TrimEnd();
                    entry.CursorPosition = Math.Max(0, Math.Min(formattedText.Length, cursorPosition));
                }
            }
            else
            {
                entry.Text = cleanedText;  
            }
        }



    }
}
