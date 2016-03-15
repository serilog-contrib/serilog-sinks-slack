using System;
using System.Collections.Generic;
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

        public SlackSink(string webhookUrl) : base(50, TimeSpan.FromSeconds(5))
        {
            _webhookUrl = webhookUrl;
        }

        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                var client = new RestClient(_webhookUrl);
                var request = new RestRequest(Method.POST);
                dynamic body = new { attachments = WrapInAttachment(logEvent).ToArray() };
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
                    CreateAttachmentField("Message", log.RenderMessage())
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
                fallback = $"Exception: {ex.Message} \n {ex.StackTrace}",
                color = GetAttachmentColor(LogEventLevel.Fatal),
                fields = new[]
                {
                    CreateAttachmentField("Message", ex.Message),
                    CreateAttachmentField("Type", ex.GetType().Name),
                    CreateAttachmentField("Stack Trace", "`"+ex.StackTrace+"`", false)
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
