using System.Collections.Generic;

namespace PlayStation_App.Core.Entities
{
    public class UserEntity
    {
        public string OnlineId { get; set; }
        public string Region { get; set; }
        public string NpId { get; set; }
        public string AvatarUrl { get; set; }
        public string PanelBgc { get; set; }
        public string PanelUrl { get; set; }
        public string AboutMe { get; set; }
        public List<string> LanguagesUsed { get; set; }
        public bool Plus { get; set; }
        public TrophySummary trophySummary { get; set; }
        public string Relation { get; set; }
        public Presence presence { get; set; }
        public string PersonalDetailSharing { get; set; }
        public bool RequestMessageFlag { get; set; }

        public PersonalDetail personalDetail { get; set; }

        public class EarnedTrophies
        {
            public int Platinum { get; set; }
            public int Gold { get; set; }
            public int Silver { get; set; }
            public int Bronze { get; set; }
        }

        public class TrophySummary
        {
            public int Level { get; set; }
            public int Progress { get; set; }
            public EarnedTrophies EarnedTrophies { get; set; }

            public int TotalTrophies { get; set; }
        }


        public class PrimaryInfo
        {
            public string Platform { get; set; }
            public string OnlineStatus { get; set; }
            public GameTitleInfo GameTitleInfo { get; set; }
            public string GameStatus { get; set; }
            public string LastOnlineDate { get; set; }
        }

        public class GameTitleInfo
        {
            public string NpTitleId { get; set; }
            public string TitleName { get; set; }
        }

        public class Presence
        {
            public PrimaryInfo PrimaryInfo { get; set; }
        }

        public class PersonalDetail
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public string FullName { get; set; }
            public string ProfilePictureUrl { get; set; }
        }
    }
}
