using System;
using System.Linq;
using System.Text.RegularExpressions;
namespace KoOrderRegister.EntryCheckers.INS
{
    public class BaseINSBehavior : LocalizedBehavior<Entry>
    {
        private string _unformattedText = "";

        protected override void SetLocalizedBehavior(Entry bindable)
        {
            
        }
        protected override void OnAttachedTo(Entry bindable) { base.OnAttachedTo(bindable); }
        protected override void OnDetachingFrom(Entry bindable) { base.OnDetachingFrom(bindable); }
        private void OnEntryTextChanged(object sender, TextChangedEventArgs e){ }
            /* 
             * Error: s  
             * 
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

                 int cursorPosition = entry.CursorPosition;
                 string newText = new string(e.NewTextValue.Where(c => char.IsDigit(c)).ToArray());

                 try
                 {
                     if (newText.Length > _unformattedText.Length)
                     {
                         // Karakter hozzáadása
                         _unformattedText = newText;
                     }
                     else if (newText.Length < _unformattedText.Length)
                     {
                         // Karakter törlése
                         int diff = _unformattedText.Length - newText.Length;
                         int removeStart = Math.Max(0, Math.Min(cursorPosition, _unformattedText.Length) - diff);
                         _unformattedText = _unformattedText.Remove(removeStart, Math.Min(diff, _unformattedText.Length - removeStart));
                     }

                     string formattedText = FormatText(_unformattedText);

                     if (entry.Text != formattedText)
                     {
                         int newCursorPosition = GetFormattedCursorPosition(_unformattedText, cursorPosition);
                         entry.Text = formattedText;
                         entry.CursorPosition = newCursorPosition;
                     }
                 }
                 catch (Exception ex)
                 {
                     // Log the exception or handle it as appropriate for your application
                     Console.WriteLine($"Error in OnEntryTextChanged: {ex.Message}");
                     // Fallback: just set the text without formatting
                     entry.Text = newText;
                     entry.CursorPosition = Math.Min(cursorPosition, newText.Length);
                 }
             }

             private string FormatText(string text)
             {
                 if (string.IsNullOrEmpty(text))
                     return string.Empty;

                 return Regex.Replace(text, @"(\d{3})(?=\d)", "$1 ").TrimEnd();
             }

             private int GetFormattedCursorPosition(string unformattedText, int unformattedPosition)
             {
                 if (string.IsNullOrEmpty(unformattedText))
                     return 0;

                 unformattedPosition = Math.Min(unformattedPosition, unformattedText.Length);
                 string formattedSubstring = FormatText(unformattedText.Substring(0, unformattedPosition));
                 return formattedSubstring.Length;
             }*/
    }
}