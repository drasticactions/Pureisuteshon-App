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
    public class SimpleActivityTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StoreActivitySimpleDataTemplate { get; set; }

        public DataTemplate ActivitySimpleDataTemplate { get; set; }

        public DataTemplate WhatsNewButtonTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item,
            DependencyObject container)
        {
            var feedItem = item as RecentActivityEntity.Feed;
            if (feedItem == null) return null;
            if (feedItem.IsMenuItem)
            {
                return WhatsNewButtonTemplate;
            }
            var uiElement = container as UIElement;
            switch (feedItem.StoryType)
            {
                case "STORE_PROMO":
                    return StoreActivitySimpleDataTemplate;
                default:
                        return ActivitySimpleDataTemplate;
            }
        }
    }
}
