using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlayStation_App.Models.Authentication
{
    public class LogInUser
    {
        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("mAccountId")]
        public string MAccountId { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("onlineId")]
        public string OnlineId { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("dateOfBirth")]
        public string DateOfBirth { get; set; }

        [JsonProperty("communityDomain")]
        public string CommunityDomain { get; set; }

        [JsonProperty("subaccount")]
        public bool Subaccount { get; set; }

        [JsonProperty("ps4Available")]
        public bool Ps4Available { get; set; }

        [JsonProperty("parentalControl")]
        public ParentalControl ParentalControl { get; set; }
    }
}
