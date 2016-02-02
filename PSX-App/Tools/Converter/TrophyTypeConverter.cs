using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace PlayStation_App.Tools.Converter
{
    public class TrophyTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Trophy/Hidden.png"));
            var trophyType = (string) value;
            switch (trophyType)
            {
                case "platinum":
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Trophy/Platinum.png"));
                case "gold":
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Trophy/Gold.png"));
                case "silver":
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Trophy/Silver.png"));
                case "bronze":
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Trophy/Bronze.png"));
                default:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Trophy/Hidden.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}