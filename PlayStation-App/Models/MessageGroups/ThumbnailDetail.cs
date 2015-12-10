
using Newtonsoft.Json;

namespace PlayStation_App.Models.MessageGroups
{

    public class ThumbnailDetail
    {

        [JsonProperty("modifierOnlineId")]
        public string ModifierOnlineId { get; set; }

        [JsonProperty("lastUpdateDate")]
        public string LastUpdateDate { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }

}
