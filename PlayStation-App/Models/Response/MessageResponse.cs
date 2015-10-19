using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Models.MessageGroups;
using PlayStation_App.Models.Messages;

namespace PlayStation_App.Models.Response
{
    public class MessageResponse
    {
        [JsonProperty("messageGroup")]
        public MessageGroup MessageGroup { get; set; }

        [JsonProperty("messages")]
        public Message[] Messages { get; set; }

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }
    }
}
