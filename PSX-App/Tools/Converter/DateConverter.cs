using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace PlayStation_App.Tools.Converter
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dateString = value as string;
            if (string.IsNullOrEmpty(dateString)) return value;
            try
            {
                DateTime date = DateTime.Parse(dateString);
                return date.ToLocalTime().ToString(CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}