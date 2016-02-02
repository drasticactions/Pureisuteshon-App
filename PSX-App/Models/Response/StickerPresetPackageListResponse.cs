using Newtonsoft.Json;
using PlayStation_App.Models.StickerPresetPackage;

namespace PlayStation_App.Models.Response
{

    public class StickerPresetPackageListResponse
    {

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("presetPackageList")]
        public PresetPackageList[] PresetPackageList { get; set; }
    }

}
