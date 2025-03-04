using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog.Sinks.Slack.Models;

namespace Serilog.Sinks.Slack
{
    public class SlackWebhookClient : IDisposable
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public async Task<bool> SendMessageAsync(string webhookUrl, Message message)
        {
            var json = JsonConvert.SerializeObject(message, _jsonSerializerSettings);
            using (var content = new StringContent(json))
            {
                var response = await _client.PostAsync(webhookUrl, content);
                return response.IsSuccessStatusCode;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
