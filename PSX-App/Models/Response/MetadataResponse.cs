using Newtonsoft.Json;
using SQLite.Net.Attributes;

namespace PlayStation_App.Models.Response
{
    public class MetadataResponse
    {
        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [PrimaryKey]
        [JsonProperty("stickerPackageId")]
        public string StickerPackageId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }
    }
}
