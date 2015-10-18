using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace PlayStation_App.Tools.Converter
{
    public class TrophyGradeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var trophyType = (string) value;
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            switch (trophyType)
            {
                case "platinum":
                    return resourceLoader.GetString("TrophyPlatinum/Text").Trim();
                case "gold":
                    return resourceLoader.GetString("TrophyGold/Text").Trim();
                case "silver":
                    return resourceLoader.GetString("TrophySilver/Text").Trim();
                case "bronze":
                    return resourceLoader.GetString("TrophyBronze/Text").Trim();
                default:
                    return resourceLoader.GetString("TrophyHidden/Text").Trim();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}