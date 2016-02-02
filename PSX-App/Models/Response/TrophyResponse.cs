using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Models.Trophies;

namespace PlayStation_App.Models.Response
{
    public class TrophyResponse
    {
        [JsonProperty("trophies")]
        public Trophy[] Trophies { get; set; }
    }
}
