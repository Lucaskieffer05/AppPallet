using System.Globalization;
using Microsoft.Maui.Controls;

namespace AppPallet.Converters
{
    public class NumericFormatBehavior : Behavior<Entry>
    {
        bool _isUpdating;

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(bindable);
        }

        // Solución: especificar que sender no es nulo usando el modificador 'object?'
        void OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;

            var entry = sender as Entry;
            if (entry == null || string.IsNullOrWhiteSpace(entry.Text)) return;

            // Elimina separadores existentes
            var clean = entry.Text.Replace(".", "").Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.CurrentCulture, out var value))
            {
                _isUpdating = true;
                entry.Text = value.ToString("#,##0.##", CultureInfo.CurrentCulture);
                entry.CursorPosition = entry.Text.Length;
                _isUpdating = false;
            }
        }
    }
}