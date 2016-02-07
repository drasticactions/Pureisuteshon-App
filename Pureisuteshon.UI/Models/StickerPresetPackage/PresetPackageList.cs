using Newtonsoft.Json;

namespace PlayStation_App.Models.StickerPresetPackage
{

    public class PresetPackageList
    {

        [JsonProperty("stickerPackageId")]
        public string StickerPackageId { get; set; }

        [JsonProperty("manifestUrl")]
        public string ManifestUrl { get; set; }
    }

}
