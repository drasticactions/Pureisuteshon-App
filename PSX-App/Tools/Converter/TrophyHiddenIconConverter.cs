using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace PlayStation_App.Tools.Converter
{
    public class TrophyHiddenIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value ?? new BitmapImage(new Uri("ms-appx:///Assets/Icons/Trophy/Hidden.png"));
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}