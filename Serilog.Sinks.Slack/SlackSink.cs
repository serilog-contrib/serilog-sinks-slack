using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Slack
{
    /// <summary>
    /// Implements <see cref="PeriodicBatchingSink"/> and provides means needed for sending Serilog log events to Slack.
    /// </summary>
    public class SlackSink : PeriodicBatchingSink
    {
        private static readonly HttpClient Client = new HttpClient();

        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private static readonly Dictionary<LogEventLevel, string> Colors = new Dictionary<LogEventLevel, string>
        {
            {LogEventLevel.Verbose, "#777"},
            {LogEventLevel.Debug, "#777"},
            {LogEventLevel.Information, "#5bc0de"},
            {LogEventLevel.Warning, "#f0ad4e"},
            {LogEventLevel.Error, "#d9534f"},
            {LogEventLevel.Fatal, "#d9534f"}
        };

        private readonly string _webhookUrl;
        private readonly string _customChannel;
        private readonly string _customUserName;
        private readonly string _customIcon;

        /// <summary>
        /// Initializes new instance of <see cref="SlackSink"/>.
        /// </summary>
        /// <param name="webhookUrl">Slack team post URI.</param>
        /// <param name="batchSizeLimit">The time to wait between checking for event batches.</param>
        /// <param name="period">The time to wait between checking for event batches..</param>
        /// <param name="customChannel">Name of Slack channel to which message should be posted.</param>
        /// <param name="customUserName">User name that will be displayed as a name of the message sender.</param>
        /// <param name="customIcon">Icon that will be used as a sender avatar.</param>
        public SlackSink(string webhookUrl, int batchSizeLimit, TimeSpan period, string customChannel = null, string customUserName = null, string customIcon = null)
            : base(batchSizeLimit, period)
        {
            _webhookUrl = webhookUrl;
            _customChannel = customChannel;
            _customUserName = customUserName;
            _customIcon = customIcon;
        }

        /// <summary>
        /// Overrides <see cref="PeriodicBatchingSink.EmitBatchAsync"/> method and uses <see cref="HttpClient"/> to post <see cref="LogEvent"/> to Slack.
        /// /// </summary>
        /// <param name="events">Collection of <see cref="LogEvent"/>.</param>
        /// <returns>Awaitable task.</returns>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                var message = CreateMessage(logEvent, _customChannel, _customUserName, _customIcon);
                var json = JsonConvert.SerializeObject(message, Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);
                await Client.PostAsync(_webhookUrl, new StringContent(json));
            }
        }

        private static dynamic CreateMessage(LogEvent logEvent, string channel, string username, string emoji)
        {
            var message = new Message
            {
                Text = logEvent.RenderMessage(),
                Channel = string.IsNullOrWhiteSpace(channel) ? string.Empty : channel,
                UserName = string.IsNullOrWhiteSpace(username) ? string.Empty : username,
                IconEmoji = string.IsNullOrWhiteSpace(emoji) ? string.Empty : emoji,
                Attachments = CreateAttachments(logEvent)
            };

            return message;
        }

        private static List<Attachment> CreateAttachments(LogEvent logEvent)
        {
            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    Fallback = $"[{logEvent.Level}]{logEvent.RenderMessage()}",
                    Color = Colors[logEvent.Level],
                    Fields = new List<Field>
                    {
                        new Field{Title = "Level", Value = logEvent.Level.ToString()},
                        new Field{Title = "Timestamp", Value = logEvent.Timestamp.ToString()}
                    }
                }
            };

            if (logEvent.Exception == null)
                return attachments;

            attachments.Add(new Attachment
            {
                Title = "Exception",
                Fallback = $"Exception: {logEvent.Exception.Message} \n {logEvent.Exception.StackTrace}",
                Color = Colors[LogEventLevel.Fatal],
                Fields = new List<Field>
                {
                    new Field{Title = "Message", Value = logEvent.Exception.Message},
                    new Field{Title = "Type", Value = "`" + logEvent.Exception.GetType().Name + "`"},
                    new Field{Title = "Stack Trace", Value = "```" + logEvent.Exception.StackTrace + "```", Short = false}
                },
                MrkdwnIn = new List<string> { "fields" }
            });

            return attachments;
        }
    }
}