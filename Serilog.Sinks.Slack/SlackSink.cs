using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System.Linq;

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

        private readonly SlackSinkOptions _options;

        /// <summary>
        /// Initializes new instance of <see cref="SlackSink"/>.
        /// </summary>
        /// <param name="options">Slack sink options object.</param>
        public SlackSink(SlackSinkOptions options)
            : base(options.BatchSizeLimit, options.Period)
        {
            _options = options;
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
                var message = CreateMessage(logEvent);
                var json = JsonConvert.SerializeObject(message, jsonSerializerSettings);
                await Client.PostAsync(_options.WebHookUrl, new StringContent(json));
            }
        }

        protected override void Dispose(bool disposing)
        {
            Client.Dispose();
            base.Dispose(disposing);
        }

        protected Message CreateMessage(LogEvent logEvent)
        {
            return new Message
            {
                Text = logEvent.RenderMessage(),
                Channel = _options.CustomChannel,
                UserName = _options.CustomUserName,
                IconEmoji = _options.CustomIcon,
                Attachments = CreateAttachments(logEvent).ToList()
            };
        }

        protected IEnumerable<Attachment> CreateAttachments(LogEvent logEvent)
        {
            // If default attachments are enabled.
            if (_options.ShowDefaultAttachments)
            {
                yield return new Attachment
                {
                    Fallback = $"[{logEvent.Level}]{logEvent.RenderMessage()}",
                    Color = _options.AttachmentColors[logEvent.Level],
                    Fields = new List<Field>
                    {
                        new Field{Title = "Level", Value = logEvent.Level.ToString()},
                        new Field{Title = "Timestamp", Value = logEvent.Timestamp.ToString()}
                    }
                };
            }

            // If there is an exception in the current event,
            // and exception attachments are enabled.
            if (logEvent.Exception != null && _options.ShowExceptionAttachments)
            {
                yield return new Attachment
                {
                    Title = "Exception",
                    Fallback = $"Exception: {logEvent.Exception.Message} \n {logEvent.Exception.StackTrace}",
                    Color = _options.AttachmentColors[LogEventLevel.Fatal],
                    Fields = new List<Field>
                    {
                        new Field{Title = "Message", Value = logEvent.Exception.Message},
                        new Field{Title = "Type", Value = "`" + logEvent.Exception.GetType().Name + "`"},
                        new Field{Title = "Stack Trace", Value = "```" + logEvent.Exception.StackTrace + "```", Short = false}
                    },
                    MrkdwnIn = new List<string> { "fields" }
                };
            }
        }
    }
}