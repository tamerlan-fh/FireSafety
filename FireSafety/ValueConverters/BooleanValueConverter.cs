using System;
using System.Globalization;
using System.Windows.Data;

namespace FireSafety.ValueConverters
{
    public class BooleanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool)) return string.Empty;

            if ((bool)value)
                return TrueValue;
            else
                return FalseValue;
        }

        private string TrueValue = "Да";
        private string FalseValue = "Нет";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is string)) return null;

            return value.ToString().ToLower() == TrueValue.ToLower();
        }
    }
}
