using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlayStation_App.Models.Messages
{
    public class SentMessageResponse
    {
        [JsonProperty("messageGroupId")]
        public string MessageGroupId { get; set; }

        [JsonProperty("messageGroupModifiedDate")]
        public string MessageGroupModifiedDate { get; set; }

        [JsonProperty("messageUid")]
        public int MessageUid { get; set; }

        [JsonProperty("sentMessageId")]
        public string SentMessageId { get; set; }
    }
}
