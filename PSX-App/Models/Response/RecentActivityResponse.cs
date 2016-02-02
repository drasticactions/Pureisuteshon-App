using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Models.RecentActivity;

namespace PlayStation_App.Models.Response
{
    public class RecentActivityResponse
    {
        [JsonProperty("feed")]
        public Feed[] Feed { get; set; }
    }
}
