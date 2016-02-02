using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using PlayStation_App.Models.Trophies;

namespace PlayStation_App.Tools.Converter
{
    public class TrophyDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as Trophy;
            if (item == null) return Visibility.Collapsed;
            if (item.ComparedUser != null)
            {
                return item.ComparedUser.EarnedDate != null
                    ? DateTime.Parse(item.ComparedUser.EarnedDate).ToLocalTime().ToString(CultureInfo.CurrentCulture)
                    : string.Empty;
            }
            if (item.FromUser != null)
            {
                return item.FromUser.EarnedDate != null
                    ? DateTime.Parse(item.FromUser.EarnedDate).ToLocalTime().ToString(CultureInfo.CurrentCulture)
                    : string.Empty;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}