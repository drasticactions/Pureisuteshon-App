using Newtonsoft.Json;
using PlayStation_App.Models.Sticker;
using SQLite.Net.Attributes;

namespace PlayStation_App.Models.Response
{
    public class StickerResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        public string Copyright { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Publisher { get; set; }

        [PrimaryKey]
        [JsonProperty("stickerPackageId")]
        public string StickerPackageId { get; set; }

        [JsonProperty("metadataUrl")]
        public string MetadataUrl { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [Ignore]
        [JsonProperty("stickerImagesBySize")]
        public StickerImagesBySize StickerImagesBySize { get; set; }
    }
}
