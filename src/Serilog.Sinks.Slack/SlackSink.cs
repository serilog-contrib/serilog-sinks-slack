using System;
using System.Collections.Generic;
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
                dynamic body = new { text = logEvent.RenderMessage() };
                request.AddJsonBody(body);
                await client.ExecuteTaskAsync(request);
            }
        }
    }
}
