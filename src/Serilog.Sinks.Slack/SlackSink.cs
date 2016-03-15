using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Slack
{
    public class SlackSink : PeriodicBatchingSink
    {
        private readonly string _webhookUrl;
        private readonly string _customChannel;
        private readonly string _customUserName;
        private readonly string _customIcon;

        public SlackSink(string webhookUrl, string customChannel = null, string customUserName = null, string customIcon = null) : base(50, TimeSpan.FromSeconds(5))
        {
            _webhookUrl = webhookUrl;
            _customChannel = customChannel;
            _customUserName = customUserName;
            _customIcon = customIcon;
        }

        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                var client = new RestClient(_webhookUrl);
                var request = new RestRequest(Method.POST);

                dynamic body = new ExpandoObject();
                body.text = logEvent.RenderMessage();
                body.attachments = WrapInAttachment(logEvent).ToArray();


                if (!string.IsNullOrWhiteSpace(_customChannel))
                    body.channel = _customChannel;
                if (!string.IsNullOrWhiteSpace(_customUserName))
                    body.username = _customUserName;
                if (!string.IsNullOrWhiteSpace(_customIcon))
                    body.icon_emoji = _customIcon;

                request.AddJsonBody(body);
                await client.ExecuteTaskAsync(request);
            }
        }

        private IEnumerable<dynamic> WrapInAttachment(LogEvent log)
        {
            var result = new List<dynamic>();
            result.Add(new
            {
                fallback = $"[{log.Level}]{log.RenderMessage()}",
                color = GetAttachmentColor(log.Level),
                fields = new[]
                {
                    CreateAttachmentField("Level", log.Level.ToString()),
                    CreateAttachmentField("Timestamp", log.Timestamp.ToString())
                }
            });

            if (log.Exception != null)
                result.Add(WrapInAttachment(log.Exception));

            return result;
        }

        private object WrapInAttachment(Exception ex)
        {
            return new
            {
                title = "Exception",
                fallback = $"Exception: {ex.Message} \n {ex.StackTrace}",
                color = GetAttachmentColor(LogEventLevel.Fatal),
                fields = new[]
                {
                    CreateAttachmentField("Message", ex.Message),
                    CreateAttachmentField("Type", "`"+ex.GetType().Name+"`"),
                    CreateAttachmentField("Stack Trace", "```"+ex.StackTrace+"```", false)
                },
                mrkdwn_in = new[] { "fields" }
            };
        }

        private object CreateAttachmentField(string title, string value, bool @short = true)
        {
            return new { title, value, @short };
        }

        private string GetAttachmentColor(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Information:
                    return "#5bc0de";
                case LogEventLevel.Warning:
                    return "#f0ad4e";
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return "#d9534f";
                default:
                    return "#777";
            }
        }
    }
}
