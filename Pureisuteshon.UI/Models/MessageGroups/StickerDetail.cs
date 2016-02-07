
using Newtonsoft.Json;

namespace PlayStation_App.Models.MessageGroups
{

    public class StickerDetail
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("manifestFileUrl")]
        public string ManifestFileUrl { get; set; }

        [JsonProperty("packageId")]
        public string PackageId { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }
    }

}
