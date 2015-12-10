using Newtonsoft.Json;

namespace PlayStation_App.Models.Sticker
{

    public class Zip
    {

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("sha1sum")]
        public string Sha1sum { get; set; }
    }

}
