using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System.Linq;
using Serilog.Formatting;

namespace Serilog.Sinks.Slack
{
    /// <summary>
    /// Implements <see cref="PeriodicBatchingSink"/> and provides means needed for sending Serilog log events to Slack.
    /// </summary>
    public class SlackSink : PeriodicBatchingSink
    {
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// These properties can be used to override options at a message level.
        /// They will not appear as attachments.
        /// </summary>
        private enum OverrideableProperties
        {
            CustomChannel,
            CustomUserName,
            CustomIcon
        }

        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly SlackSinkOptions _options;
        private readonly ITextFormatter _textFormatter;

        /// <summary>
        /// Initializes new instance of <see cref="SlackSink"/>.
        /// </summary>
        /// <param name="options">Slack sink options object.</param>
        /// <param name="textFormatter">Formatter used to convert log events to text.</param>
        public SlackSink(SlackSinkOptions options, ITextFormatter textFormatter)
            : base(options.BatchSizeLimit, options.Period)
        {
            _options = options;
            _textFormatter = textFormatter;
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
                if (logEvent.Level < _options.MinimumLogEventLevel) continue;
                var message = CreateMessage(logEvent);
                var json = JsonConvert.SerializeObject(message, jsonSerializerSettings);
                await Client.PostAsync(_options.WebHookUrl, new StringContent(json));
            }
        }

        protected override void Dispose(bool disposing)
        {
            // Need to call base dispose first to flush last of messages *before* disposing of client.
            base.Dispose(disposing);
            Client.Dispose();
        }

        protected Message CreateMessage(LogEvent logEvent)
        {
            var textWriter = new StringWriter();
            _textFormatter.Format(logEvent, textWriter);

            return new Message
            {
                Text = textWriter.ToString(),
                Channel = GetPropertyFromException(logEvent, OverrideableProperties.CustomChannel, _options.CustomChannel),
                UserName = GetPropertyFromException(logEvent, OverrideableProperties.CustomUserName, _options.CustomUserName),
                IconEmoji = GetPropertyFromException(logEvent, OverrideableProperties.CustomIcon, _options.CustomIcon),
                Attachments = CreateAttachments(logEvent).ToList()
            };
        }

        private static string GetPropertyFromException(LogEvent logEvent, OverrideableProperties overrideableProperty, string defaultValue)
        {
            var overrideablePropertyName = Enum.GetName(typeof(OverrideableProperties), overrideableProperty);
            return logEvent.Properties.TryGetValue(overrideablePropertyName, out var logEventPropertyValue) 
                ? logEventPropertyValue.ToString("l", null) 
                : defaultValue;
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
                        new Field{Title = "Level", Value = logEvent.Level.ToString(), Short = _options.DefaultAttachmentsShortFormat},
                        new Field{Title = "Timestamp", Value = logEvent.Timestamp.ToString(), Short = _options.DefaultAttachmentsShortFormat}
                    }
                };
            }

            if (_options.ShowPropertyAttachments)
            {
                var fields = new List<Field>();
                var overrideablePropertyNames = Enum.GetNames(typeof(OverrideableProperties));

                var stringWriter = new StringWriter();
                foreach (var property in logEvent.Properties.Where(kvp=>!overrideablePropertyNames.Contains(kvp.Key)))
                {
                    property.Value.Render(stringWriter);
                    var field = new Field
                    {
                        Title = property.Key,
                        Value = stringWriter.ToString(),
                        Short = _options.PropertyAttachmentsShortFormat
                    };
                    fields.Add(field);

                    stringWriter.GetStringBuilder().Clear();
                }

                yield return new Attachment
                {
                    Fallback = $"[{logEvent.Level}]{logEvent.RenderMessage()}",
                    Color = _options.AttachmentColors[logEvent.Level],
                    Fields = fields
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
                        new Field{Title = "Type", Value = $"`{logEvent.Exception.GetType().Name}`"},
                        new Field{Title = "Stack Trace", Value = $"```{logEvent.Exception.StackTrace}```", Short = false},
                        new Field{Title = "Exception", Value = $"```{logEvent.Exception.ToString()}```", Short = false}
                    },
                    MrkdwnIn = new List<string> { "fields" }
                };
            }
        }
    }
}