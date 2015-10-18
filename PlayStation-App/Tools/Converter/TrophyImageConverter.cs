using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using PlayStation_App.Models.RecentActivity;

namespace PlayStation_App.Tools.Converter
{
    public class TrophyImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var targets = value as Target2[];
            if (targets == null)
            {
                var targetOne = value as Target[];
                return targetOne == null ? string.Empty : ConvertTarget(targetOne);
            }
            else
            {
                return ConvertTarget2(targets);
            }
         
        }

        public string ConvertTarget2(Target2[] targets)
        {
            var trophyImageObject = targets.FirstOrDefault(node => node.Type == "TROPHY_IMAGE_URL");
            return trophyImageObject != null ? trophyImageObject.Meta : string.Empty;
        }

        public string ConvertTarget(Target[] targets)
        {
            var trophyImageObject = targets.FirstOrDefault(node => node.Type == "TROPHY_IMAGE_URL");
            return trophyImageObject != null ? trophyImageObject.Meta : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
