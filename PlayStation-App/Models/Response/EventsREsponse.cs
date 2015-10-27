using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Models.Events;

namespace PlayStation_App.Models.Response
{
    public class EventsResponse
    {
        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("events")]
        public Event[] Events { get; set; }
    }
}
