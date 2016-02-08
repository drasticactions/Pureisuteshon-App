using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PlayStation_App.Tools.Converter
{
    public class BooleanToVisibilityInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value == false) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
