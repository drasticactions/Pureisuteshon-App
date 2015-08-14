using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using PlayStation_App.Core.Entities;

namespace PlayStation_App.Tools.TemplateSelector
{
    class SimpleLiveFeedTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SimpleLiveBroadcastButtonTemplate { get; set; }

        public DataTemplate SimpleLiveBroadcastTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item,
            DependencyObject container)
        {
            var feedItem = item as LiveBroadcastEntity;
            if (feedItem == null) return null;
            var uiElement = container as UIElement;

            if (feedItem.IsMenuItem)
            {
                return SimpleLiveBroadcastButtonTemplate;
            }

            return SimpleLiveBroadcastTemplate;
        }
    }

}
