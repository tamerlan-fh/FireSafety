using System;
using System.Globalization;
using System.Windows.Data;

namespace FireSafety.ValueConverters
{
    public class DoubleRoundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is double)) return 0;

            int decimals = 3;
            if (parameter != null)
                int.TryParse(parameter.ToString(), out decimals);
            if (decimals < 0)
                decimals = 3;

            double val = 0;
            if (!double.TryParse(value.ToString(), out val)) return 0;

            return Math.Round(val, decimals);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
