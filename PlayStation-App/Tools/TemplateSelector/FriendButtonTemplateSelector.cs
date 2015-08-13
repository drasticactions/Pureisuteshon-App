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
    public class FriendButtonTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FriendButtonTemplate { get; set; }

        public DataTemplate SimpleFriendTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item,
            DependencyObject container)
        {
            var feedItem = item as FriendsEntity.Friend;
            if (feedItem == null) return null;
            var uiElement = container as UIElement;

            if (feedItem.IsMenuItem)
            {
                VariableSizedWrapGrid.SetRowSpan(uiElement, 3);
                VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                return FriendButtonTemplate;
            }

            VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
            VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
            return SimpleFriendTemplate;
        }
    }
}
