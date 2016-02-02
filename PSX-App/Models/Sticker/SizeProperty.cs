using Newtonsoft.Json;

namespace PlayStation_App.Models.Sticker
{
    public class SizeProperty
    {
        public string Size { get; set; }

        [JsonProperty("zip")]
        public Zip Zip { get; set; }

        [JsonProperty("urls")]
        public string[] Urls { get; set; }
    }
}
