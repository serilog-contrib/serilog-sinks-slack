using System.Collections.Generic;
using Newtonsoft.Json;

namespace Serilog.Sinks.Slack.Models
{
    public class Attachment
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("fallback")]
        public string Fallback { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }

        [JsonProperty("mrkdwn_in")]
        public List<string> MrkdwnIn { get; set; }
    }
}