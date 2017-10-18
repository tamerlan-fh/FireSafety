using System;
using System.Globalization;
using System.Windows.Data;

namespace FireSafety.ValueConverters
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromMinutes((double)value).ToString(@"mm\:ss\.fff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan result;
            if (TimeSpan.TryParse(value.ToString(), out result))
                return result;
            return value;
        }
    }
}
