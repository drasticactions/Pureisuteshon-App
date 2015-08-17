using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayStation_App.Core.Entities
{
    public class LiveBroadcastEntity
    {
        public bool IsMenuItem { get; set; }
        public string Title { get; set; }

        public string From { get; set; }

        public string Description { get; set; }

        public string UserName { get; set; }

        public string GameTitle { get; set; }

        public string GameMetadata { get; set; }

        public bool IsOnline { get; set; }

        public string OnlineTime { get; set; }

        public string BroadcastId { get; set; }

        public string Platform { get; set; }

        public string Language { get; set; }

        public int Viewers { get; set; }

        public int SocialStream { get; set; }

        public string PreviewThumbnail { get; set; }

        public string Service { get; set; }

        public string Url { get; set; }

        public void ParseFromNicoNico(NicoNicoEntity.Program program)
        {
            try
            {
                Title = program.title;
                Service = "/Assets/Icons/Stream/Nico.png";
                Description = program.description;
                UserName = program.user.name;
                GameTitle = program.sce.title_name;
                Platform = "PS4";
                Viewers = program.view_num;
                PreviewThumbnail = program.thumbnail_url;
                Url = "http://live.nicovideo.jp/watch/" + program.id;
            }
            catch (Exception)
            {
                return;
            }
        }

        public void ParseFromTwitch(TwitchEntity.Stream twitchStream)
        {
            try
            {
                Title = twitchStream.status;
                Service = "/Assets/Icons/Stream/Twitch.png";
                Description = twitchStream.name;
                UserName = twitchStream.sce_user_online_id;
                GameTitle = twitchStream.sce_title_name;
                OnlineTime = DateTime.Parse(twitchStream.stream_up).ToLocalTime().ToString();
                BroadcastId = twitchStream.broadcast_id as string;
                Platform = twitchStream.sce_platform;
                Language = twitchStream.sce_title_language;
                GameMetadata = twitchStream.sce_title_metadata;
                PreviewThumbnail = twitchStream.preview;
                Viewers = twitchStream.viewers;
                Url = string.Format("http://www.twitch.tv/{0}", UserName);
            }
            catch (Exception)
            {
                return;
            }
        }

        public void ParseFromUstream(UstreamEntity.Item ustreamEntity)
        {
            try
            {
                Service = "/Assets/Icons/Stream/Ustream.png";
                Title = ustreamEntity.media.title;
                PreviewThumbnail = ustreamEntity.media.thumbnail.live;
                Description = ustreamEntity.media.description;
                GameTitle = ustreamEntity.media.description;
                Viewers = ustreamEntity.media.stats.viewer;
                SocialStream = ustreamEntity.media.stats.socialstream;
                var testDate = new DateTime().AddSeconds(ustreamEntity.media.stream_started_at);
                OnlineTime = testDate.ToLocalTime().ToString();
                Url = string.Format("http://www.ustream.tv/channel/id/{0}", ustreamEntity.media.id);
            }
            catch (Exception)
            {
                return;
            }
        }

    }
}
