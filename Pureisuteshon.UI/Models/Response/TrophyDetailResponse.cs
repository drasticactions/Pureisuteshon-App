using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Models.TrophyDetail;

namespace PlayStation_App.Models.Response
{
    public class TrophyDetailResponse
    {
        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("trophyTitles")]
        public TrophyTitle[] TrophyTitles { get; set; }
    }
}
