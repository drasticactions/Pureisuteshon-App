namespace PlayStation_App.Core.Tools
{
    public class EndPoints
    {
        public class UstreamUrlConstants
        {
            public const string FilterBase = "filter[{0}]";

            public const string Platform = "platform";

            public const string Type = "type";

            public const string PlatformPs4 = "PS4";

            public const string Interactive = "interactive";

            public const string Sort = "sort";
        }

        public const string UstreamBaseUrl = "https://ps4api.ustream.tv/media.json?";

        public const string TwitchBaseUrl = "https://api.twitch.tv/api/orbis/streams?";

        public const string NicoNicoBaseUrl = "http://edn.live.nicovideo.jp/api/v1.0/programs?";

        public const string ConsumerKey = "4db3729d-4591-457a-807a-1cf01e60c3ac";

        public const string ConsumerSecret = "criemouwIuVoa4iU";

        public const int DefaultTimeoutInMilliseconds = 60000;

        public const string RecentlyPlayed =
    "https://activity.api.np.km.playstation.net/activity/api/v1/users/{0}/recentlyplayed?";

        public const string FriendMeUrl =
    "https://friendme.sonyentertainmentnetwork.com/friendme/api/v1/c2s/users/me/friendrequest";

        public const string RequestMessage =
           "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}/requestMessage";

        public const string SendFriendRequest =
          "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}?requestMessage={3}";

        public const string SendNameRequest =
       "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}/personalDetailSharing";

        public const string User =
            "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/profile?fields=@default,relation,onlineId,presence,avatarUrl,plus,personalDetail,trophySummary";

        public const string OauthToken = "https://auth.api.sonyentertainmentnetwork.com/2.0/oauth/token";

        public const string VerifyUser = "https://vl.api.np.km.playstation.net/vl/api/v1/mobile/users/me/info";

        public const string LoginPost = "https://auth.api.sonyentertainmentnetwork.com/login.do";

        public const string DenyAddFriend =
    "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}";

        public const string RecentActivity =
            "https://activity.api.np.km.playstation.net/activity/api/v1/users/{0}/{1}/{2}?filters=PURCHASED&filters=RATED&filters=VIDEO_UPLOAD&filters=SCREENSHOT_UPLOAD&filters=PLAYED_GAME&filters=WATCHED_VIDEO&filters=TROPHY&filters=BROADCASTING&filters=LIKED&filters=PROFILE_PIC&filters=FRIENDED&filters=CONTENT_SHARE";

        public const string FriendList =
           "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList?fields=@default,relation,onlineId,avatarUrl,plus,personalDetail,trophySummary&sort=onlineId&avatarSize=m&offset={2}&limit=32";



        public const string Login =
            "https://auth.api.sonyentertainmentnetwork.com/2.0/oauth/authorize?response_type=code&returnAuthCode=true&service_entity=urn:service-entity:psn&client_id=4db3729d-4591-457a-807a-1cf01e60c3ac&redirect_uri=com.playstation.PlayStationApp://redirect&scope=psn:sceapp";
    }
}
