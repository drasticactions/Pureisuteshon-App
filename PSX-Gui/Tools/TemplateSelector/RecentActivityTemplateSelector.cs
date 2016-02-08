using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using PlayStation_App.Models.RecentActivity;
using PlayStation_App.Models.Response;

namespace PlayStation_App.Tools.TemplateSelector
{
    public class RecentActivityTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PreviousActivityDataTemplate { get; set; }

        public DataTemplate ReloadActivityDataTemplate { get; set; }

        public DataTemplate LoadMoreActivityDataTemplate { get; set; }
        public DataTemplate StoreActivityDataTemplate { get; set; }

        public DataTemplate MultiplePeoplePlayActivityDataTemplate { get; set; }

        public DataTemplate ActivityDataTemplate { get; set; }

        public DataTemplate FriendedActivityDataTemplate { get; set; }

        public DataTemplate PlayedGameActivityDataTemplate { get; set; }

        public DataTemplate MultipleTrophyActivityDataTemplate { get; set; }

        public DataTemplate TrophyActivityDataTemplate { get; set; }

        public DataTemplate BroadcastDataTemplate { get; set; }

        public DataTemplate ProfilePicDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item,
            DependencyObject container)
        {
            var feedItem = item as Feed;
            if (feedItem == null) return null;

            var uiElement = container as UIElement;
            if (feedItem.IsNextButton)
            {
                VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
                VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                return LoadMoreActivityDataTemplate;
            }
            if (feedItem.IsPreviousButton && feedItem.IsReloadButton)
            {
                VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
                VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                return ReloadActivityDataTemplate;
            }
            if (feedItem.IsPreviousButton)
            {
                VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
                VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                return PreviousActivityDataTemplate;
            }
            switch (feedItem.StoryType)
            {
                case "PROFILE_PIC":
                    VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
                    VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                    return ProfilePicDataTemplate;
                case "BROADCASTING":
                    VariableSizedWrapGrid.SetRowSpan(uiElement, 2);
                    VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                    return BroadcastDataTemplate;
                case "TROPHY":
                    if (feedItem.CondensedStories != null)
                    {
                        VariableSizedWrapGrid.SetRowSpan(uiElement, 2);
                        VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                        return MultipleTrophyActivityDataTemplate;
                    }
                    else
                    {
                        VariableSizedWrapGrid.SetRowSpan(uiElement, 2);
                        VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                        return TrophyActivityDataTemplate;
                    }
                case "STORE_PROMO":
                    VariableSizedWrapGrid.SetRowSpan(uiElement, 2);
                    VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                    return StoreActivityDataTemplate;
                case "FRIENDED":
                    VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
                    VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                    return FriendedActivityDataTemplate;
                case "PLAYED_GAME":
                    if (feedItem.CondensedStories != null)
                    {
                        VariableSizedWrapGrid.SetRowSpan(uiElement, 2);
                        VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                        return MultiplePeoplePlayActivityDataTemplate;
                    }
                    else
                    {
                        VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
                        VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                        return PlayedGameActivityDataTemplate;
                    }
                case "RATED":
                    if (feedItem.CondensedStories != null)
                    {
                        VariableSizedWrapGrid.SetRowSpan(uiElement, 2);
                        VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                        return MultiplePeoplePlayActivityDataTemplate;
                    }
                    else
                    {
                        VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
                        VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                        return PlayedGameActivityDataTemplate;
                    }
                default:
                    if (feedItem.CondensedStories != null)
                    {
                        VariableSizedWrapGrid.SetRowSpan(uiElement, 2);
                        VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                        return MultiplePeoplePlayActivityDataTemplate;
                    }
                    else
                    {
                        VariableSizedWrapGrid.SetRowSpan(uiElement, 1);
                        VariableSizedWrapGrid.SetColumnSpan(uiElement, 1);
                        return PlayedGameActivityDataTemplate;
                    }
            }
        }

        public int StoryHeight(int length)
        {
            if (length < 100)
            {
                return 3;
            }

            if (length < 150)
            {
                return 3;
            }

            return 4;
        }
    }
}
