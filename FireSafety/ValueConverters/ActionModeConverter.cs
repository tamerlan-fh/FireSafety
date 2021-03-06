﻿using FireSafety.VisualModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FireSafety.ValueConverters
{
    public class ActionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;

            return (ActionMode)value == (ActionMode)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return null;

            if ((bool)value)
                return (ActionMode)parameter;
            return null;
        }
    }
}
