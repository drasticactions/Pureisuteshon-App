using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;

namespace PlayStation_App.Core.Entities
{
    public class RecentActivityEntity
    {
        public class Target
        {
            public string Meta { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
        }

        public class CondensedStory
        {
            public string Caption { get; set; }
            public List<Target> Targets { get; set; }
            public string CaptionTemplate { get; set; }
            public List<CaptionComponent> CaptionComponents { get; set; }
            public string StoryId { get; set; }
            public string StoryType { get; set; }
            public Source Source { get; set; }
            public string SmallImageUrl { get; set; }
            public string SmallImageAspectRatio { get; set; }
            public string LargeImageUrl { get; set; }
            public string ThumbnailImageUrl { get; set; }
            public DateTime Date { get; set; }
            public double Relevancy { get; set; }
            public int CommentCount { get; set; }
            public int LikeCount { get; set; }
            public string TitleId { get; set; }
            public string ProductId { get; set; }
            public string ProductUrl { get; set; }
            public bool Liked { get; set; }
            public string ServiceProviderName { get; set; }
            public bool Reshareable { get; set; }
        }

        public class CaptionComponent
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Source
        {
            public string Meta { get; set; }
            public string Type { get; set; }
            public string ImageUrl { get; set; }
        }

        public class Param
        {
            public string Meta { get; set; }
            public string Type { get; set; }
        }

        public class Action
        {
            public string Type { get; set; }
            public string Uri { get; set; }
            public string Platform { get; set; }
            public string ButtonCaption { get; set; }
            public string ImageUrl { get; set; }
        }

        public class Feed
        {
            public string TitleName
            {
                get
                {
                    return !Targets.Any() ? string.Empty : Targets.FirstOrDefault(node => node.Type == "TITLE_NAME")?.Meta;
                }
            }
            public string ShortGameTitle
            {
                get
                {
                    return !Targets.Any() ? string.Empty : Targets.FirstOrDefault(node => node.Type == "SHORT_PRODUCT_NAME")?.Meta;
                }
            }
            public string GameTitle
            {
                get
                {
                    return !Targets.Any() ? string.Empty : Targets.FirstOrDefault(node => node.Type == "PRODUCT_NAME")?.Meta;
                }
            }
            public bool IsMenuItem { get; set; }
            public string Caption { get; set; }

            public List<CondensedStory> CondensedStories { get; set; }

            public List<Target> Targets { get; set; }
            public string CaptionTemplate { get; set; }
            public List<CaptionComponent> CaptionComponents { get; set; }
            public List<Param> @Params { get; set; }
            public List<Action> Actions { get; set; }
            public string StoryId { get; set; }
            public string StoryType { get; set; }
            public string StoryComment { get; set; }
            public Source Source { get; set; }
            public string SmallImageUrl { get; set; }
            public string SmallImageAspectRatio { get; set; }
            public string LargeImageUrl { get; set; }
            public string ThumbnailImageUrl { get; set; }
            public DateTime Date { get; set; }

            public string FormattedDate => Date.Humanize();
            public double Relevancy { get; set; }
            public int CommentCount { get; set; }
            public int LikeCount { get; set; }
            public string TitleId { get; set; }
            public string ProductId { get; set; }
            public string ProductUrl { get; set; }
            public bool Liked { get; set; }
            public string ServiceProviderName { get; set; }
            public string ServiceProviderImageUrl { get; set; }
            public bool Reshareable { get; set; }
        }
        public List<Feed> feed { get; set; }
    }
}
