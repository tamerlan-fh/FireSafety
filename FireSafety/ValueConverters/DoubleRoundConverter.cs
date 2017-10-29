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

            if (parameter == null)
                return Math.Round((double)value, 3);
            else
            {
                int decimals = 3;
                int.TryParse((string)parameter, out decimals);
                return Math.Round((double)value, decimals);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
