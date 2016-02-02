using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace PlayStation_App.Tools.Converter
{
    public class OnlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var colorValue = (string)value;
            if (colorValue.Equals("online"))
            {
                return "/Assets/Icons/Friend/Friend_Online.png";
            }
            return colorValue.Equals("offline") ? "/Assets/Icons/Friend/Friend_Offline.png" : "/Assets/Icons/Friend/Friend_Standby.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
