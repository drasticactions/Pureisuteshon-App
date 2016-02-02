using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using PlayStation_App.Models.RecentActivity;

namespace PlayStation_App.Tools.Converter
{
    public class BroadcastImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var targets = value as Param[];
            if (targets == null)
            {
                return string.Empty;
            }
            var results = targets.FirstOrDefault(node => node.Type == "SMALL_IMAGE_URL_HTTP");
            return results != null ? results.Meta : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
