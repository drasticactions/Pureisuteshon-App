using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlayStation_App.Models.Sticker
{
    public class StickerSelection
    {
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("manifestFileUrl")]
        public string ManifestUrl { get; set; }

        [JsonProperty("packageId")]
        public string PackageId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
