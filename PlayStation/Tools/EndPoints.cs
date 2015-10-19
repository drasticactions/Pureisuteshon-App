namespace PlayStation.Tools
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

        public const string TrophyDetailList =
    "https://{0}-tpy.np.community.playstation.net/trophy/v1/trophyTitles/{1}/trophyGroups/all/trophies?fields=@default,trophyRare,trophyEarnedRate&npLanguage={2}&comparedUser={3}&fromUser={4}";

        public const string TrophyList =
            "https://{0}-tpy.np.community.playstation.net/trophy/v1/trophyTitles?fields=%40default&npLanguage={1}&platform=PS3%2CPSVITA%2CPS4&offset={2}&limit=64&comparedUser={3}&fromUser={4}";

        public const string RequestMessage =
           "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}/requestMessage";

        public const string SendFriendRequest =
          "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}?requestMessage={3}";

        public const string SendNameRequest =
       "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}/personalDetailSharing";

        public const string UserAvatars = "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/profile?fields=avatarUrls";

        public const string User =
            "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/profile?fields=onlineId,aboutMe,languagesUsed,plus,@personalDetail,avatarUrls,presence,isOfficiallyVerified,relation,requestMessageFlag,trophySummary,npTitleIconUrl,mutualFriendsCount&avatarSizes=m,xl&profilePictureSizes=m,xl&languagesUsedLanguageSet=set3&psVitaTitleIcon=circled&titleIconSize=s&aboutMeType=1";

        public const string OauthToken = "https://auth.api.sonyentertainmentnetwork.com/2.0/oauth/token";

        public const string VerifyUser = "https://vl.api.np.km.playstation.net/vl/api/v1/mobile/users/me/info";

        public const string LoginPost = "https://auth.api.sonyentertainmentnetwork.com/login.do";

        public const string DenyAddFriend =
    "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList/{2}";

        public const string RecentActivity =
            "https://activity.api.np.km.playstation.net/activity/api/v1/users/{0}/{1}/{2}?filters=PURCHASED&filters=RATED&filters=VIDEO_UPLOAD&filters=SCREENSHOT_UPLOAD&filters=PLAYED_GAME&filters=WATCHED_VIDEO&filters=TROPHY&filters=BROADCASTING&filters=LIKED&filters=PROFILE_PIC&filters=FRIENDED&filters=CONTENT_SHARE";

        public const string FriendList =
           "https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/friendList?fields=@default,relation,onlineId,avatarUrl,plus,personalDetail,trophySummary&sort=onlineId&avatarSize=m&offset={2}&limit=32";

        public const string MessageGroup2 =
    "https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/messages?fields=@default%2CmessageGroup%2Cbody&npLanguage={2}";

        public const string MessageGroup =
            "https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/users/{1}/messageGroups?fields=@default%2CmessageGroupId%2CmessageGroupDetail%2CtotalUnseenMessages%2CtotalMessages%2ClatestMessage&npLanguage={2}";

        public const string MessageContent =
            "https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/messages/{2}?contentKey={3}&npLanguage={4}";

        public const string ClearMessages =
            "https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/messages?messageUid={2}";

        public const string ClearNotification =
            "https://{0}-ntl.np.community.playstation.net/notificationList/v1/users/{1}/notifications/{2}/{3}";

        public const string DeleteThread =
            "https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/users/{2}";

        public const string CreatePost =
            "https://{0}-gmsg.np.community.playstation.net/groupMessaging/v1/messageGroups/{1}/messages";

        public const string MyEvents =
            "https://{0}-evnt-edge.np.community.playstation.net/eventsClient/v1/users/me/myEvents?fields=eventId,eventType,eventName,lastUpdate,duration,registeredUserCount,bannerImageUrl&targetEvents={1}&limit=100";

        public const string FeaturedEvents = "https://{0}-evnt-edge.np.community.playstation.net/eventsClient/v1/users/me/featuredEvents?fields=eventId,eventType,eventName,lastUpdate,duration,registeredUserCount,bannerImageUrl&targetEvents={1}&limit=6";

        public const string Events = "https://{0}-evnt-edge.np.community.playstation.net/eventsClient/v1/events?sort={1}&fields=eventId,eventType,eventName,lastUpdate,duration,registeredUserCount,bannerImageUrl&targetEvents=all";

        public const string EventTitles =
            "https://{0}-evnt-edge.np.community.playstation.net/eventsClient/v1/titles?sort={1}&fields=npTitleDetail,totalEventCount,totalRegisteredUserCount&targetTitles=all";

        public const string EventTitleDetails = "https://{0}-evnt-edge.np.community.playstation.net/eventsClient/v1/titles/{1}/events?sort={2}&fields=eventId,eventType,eventName,lastUpdate,duration,registeredUserCount,bannerImageUrl&targetEvents=all";

        public const string EventDetail = "https://{0}-evnt-edge.np.community.playstation.net/eventsClient/v1/events/{1}?fields=eventId,eventType,npCommunicationId,npTitleIds,userStatus,registeredUserCount,lastUpdate,closed,eventDetail,eventData,inGameDetail,officialBroadCastDetail,shareFactoryDetail";

        public const string AddRemoveEvent = "https://{0}-evnt-edge.np.community.playstation.net/eventsClient/v1/events/{1}/registeredUsers/{2}";

        public const string EventDetailFriends = "https://{0}-evnt-edge.np.community.playstation.net/eventsClient/v1/events/{1}/registeredUsers?targetUsers=friends&offset={2}&limit={3}";

        public const string LiveBroadcast =
            "https://ugc.api.playstation.com/distributor/v1/broadcasts?q=*:*&sort={1}&start=0&rows=80&language={2}&fq={0}&fl=serviceType,countOfViewers,smallStreamPreviewImage,countOfComments,storyId,sceUserOnlineId,sceTitleName,sceTitleProductId,sceTitleId,title,description,id,channelId,eventId";

        public const string EventLiveBroadcast =
            "https://ugc.api.playstation.com/distributor/v1/broadcasts?q=*:*&sort=countOfViewers%20desc&start=0&rows=80&language=all&fq=serviceType:YTLIVE%20OR%20serviceType:USTREAM%20OR%20serviceType:TWITCH%20OR%20serviceType:NICOLIVE&fq=eventId:{0}&fl=serviceType,countOfViewers,smallStreamPreviewImage,countOfComments,storyId,sceUserOnlineId,sceTitleName,sceTitleProductId,sceTitleId,title,description,id,channelId,eventId";
        public const string Login =
            "https://auth.api.sonyentertainmentnetwork.com/2.0/oauth/authorize?response_type=code&returnAuthCode=true&service_entity=urn:service-entity:psn&client_id=4db3729d-4591-457a-807a-1cf01e60c3ac&redirect_uri=com.playstation.PlayStationApp://redirect&scope=psn:sceapp";
    }
}
