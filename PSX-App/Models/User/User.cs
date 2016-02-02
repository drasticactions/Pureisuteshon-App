using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlayStation_App.Models.User
{
    public class User
    {
        [JsonProperty("onlineId")]
        public string OnlineId { get; set; }

        [JsonProperty("avatarUrls")]
        public AvatarUrl[] AvatarUrls { get; set; }

        [JsonProperty("aboutMe")]
        public string AboutMe { get; set; }

        [JsonProperty("languagesUsed")]
        public string[] LanguagesUsed { get; set; }

        [JsonProperty("plus")]
        public int Plus { get; set; }

        [JsonProperty("trophySummary")]
        public TrophySummary TrophySummary { get; set; }

        [JsonProperty("relation")]
        public string Relation { get; set; }

        [JsonProperty("presence")]
        public Presence Presence { get; set; }

        [JsonProperty("personalDetail")]
        public PersonalDetail PersonalDetail { get; set; }

        [JsonProperty("usePersonalDetailInGame")]
        public bool UsePersonalDetailInGame { get; set; }

        [JsonProperty("isOfficiallyVerified")]
        public bool IsOfficiallyVerified { get; set; }
    }
}
