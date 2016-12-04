using System.Collections.Generic;
using Newtonsoft.Json;

namespace Serilog.Sinks.Slack
{
    public class Message
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("icon_emoji")]
        public string IconEmoji { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }
    }
}