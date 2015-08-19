using System.Collections.Generic;

namespace PlayStation_App.Core.Entities
{
    public class TrophyEntity
    {
        public class DefinedTrophies
        {
            public int Bronze { get; set; }
            public int Silver { get; set; }
            public int Gold { get; set; }
            public int Platinum { get; set; }
        }

        public class EarnedTrophies
        {
            public int Bronze { get; set; }
            public int Silver { get; set; }
            public int Gold { get; set; }
            public int Platinum { get; set; }
        }

        public class FromUser
        {
            public string OnlineId { get; set; }
            public int Progress { get; set; }
            public EarnedTrophies EarnedTrophies { get; set; }
            public bool HiddenFlag { get; set; }
            public string LastUpdateDate { get; set; }
        }

        public class ComparedUser
        {
            public string OnlineId { get; set; }
            public int Progress { get; set; }
            public EarnedTrophies EarnedTrophies { get; set; }
            public string LastUpdateDate { get; set; }
        }

        public class TrophyTitle
        {
            public string NpCommunicationId { get; set; }
            public string TrophyTitleName { get; set; }
            public string TrophyTitleDetail { get; set; }
            public string TrophyTitleIconUrl { get; set; }
            public string TrophyTitlePlatfrom { get; set; }
            public bool HasTrophyGroups { get; set; }
            public DefinedTrophies DefinedTrophies { get; set; }
            public FromUser FromUser { get; set; }
            public ComparedUser ComparedUser { get; set; }
        }
        public int TotalResults { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public List<TrophyTitle> TrophyTitles { get; set; }
    }
}
