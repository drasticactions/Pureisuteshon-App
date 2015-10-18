using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace PlayStation_App.Tools.Converter
{
    public class TrophyRareConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var trophyValue = (int) value;
            var baseUri = new Uri("ms:appx//");
            switch (trophyValue)
            {
                case 0:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Rare/phone_trophy_rareness_ultraRare.png"));
                case 1:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Rare/phone_trophy_rareness_rare.png"));
                case 2:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Rare/phone_trophy_rareness_uncommon.png"));
                case 3:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Rare/phone_trophy_rareness_common.png"));
                case 4:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Icons/Rare/phone_trophy_rareness_common.png"));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}