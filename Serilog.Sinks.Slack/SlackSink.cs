using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using Serilog.Sinks.Slack.Helpers;
using Serilog.Sinks.Slack.Models;

namespace Serilog.Sinks.Slack
{
    /// <summary>
    /// Implements <see cref="ILogEventSink"/>, <see cref="IBatchedLogEventSink"/>, <see cref="IDisposable"/> and provides means needed for sending Serilog log events to Slack.
    /// </summary>
    public class SlackSink : ILogEventSink, IBatchedLogEventSink, IDisposable
    {
        private readonly HttpClient _client = new HttpClient();

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly SlackSinkOptions _options;
        private readonly ITextFormatter _textFormatter;
        private readonly PeriodicBatchingSink _periodicBatchingSink;
        private bool _disposed = false;

#if NETFRAMEWORK
        static SlackSink()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }
#endif

        /// <summary>
        /// Initializes new instance of <see cref="SlackSink"/>.
        /// </summary>
        /// <param name="options">Slack sink options object.</param>
        /// <param name="textFormatter">Formatter used to convert log events to text.</param>
        public SlackSink(SlackSinkOptions options, ITextFormatter textFormatter)
        {
            _options = options;
            _textFormatter = textFormatter;
            _periodicBatchingSink = new PeriodicBatchingSink(this, options.ToPeriodicBatchingSinkOptions());
        }

        /// <summary>
        /// Implements <see cref="ILogEventSink.Emit"/> method and forwards the <see cref="LogEvent"/> to the underlying <see cref="PeriodicBatchingSink"/>.
        /// </summary>
        /// <param name="logEvent">The <see cref="LogEvent"/>.</param>
        void ILogEventSink.Emit(LogEvent logEvent)
        {
            _periodicBatchingSink.Emit(logEvent);
        }

        /// <summary>
        /// Implements <see cref="IBatchedLogEventSink.EmitBatchAsync"/> method and uses <see cref="HttpClient"/> to post <see cref="LogEvent"/> to Slack.
        /// </summary>
        /// <param name="events">Collection of <see cref="LogEvent"/>.</param>
        /// <returns>Awaitable task.</returns>
        async Task IBatchedLogEventSink.EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                if (logEvent.Level < _options.MinimumLogEventLevel) continue;
                var message = CreateMessage(logEvent);
                var json = JsonConvert.SerializeObject(message, _jsonSerializerSettings);
                await _client.PostAsync(_options.WebHookUrl, new StringContent(json));
            }
        }

        /// <summary>
        /// Implements <see cref="IBatchedLogEventSink.OnEmptyBatchAsync"/> method and performs a no-op.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        Task IBatchedLogEventSink.OnEmptyBatchAsync()
        {
            return Task.Delay(0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _periodicBatchingSink.Dispose();
                _client.Dispose();
            }

            _disposed = true;
        }

        protected Message CreateMessage(LogEvent logEvent)
        {
            using (var textWriter = new StringWriter())
            {
                _textFormatter.Format(logEvent, textWriter);

                return new Message
                {
                    Text = textWriter.ToString(),
                    Channel = _options.CustomChannel,
                    UserName = _options.CustomUserName,
                    IconEmoji = _options.CustomIcon,
                    Attachments = CreateAttachments(logEvent).ToList()
                };
            }
        }

        protected IEnumerable<Attachment> CreateAttachments(LogEvent logEvent)
        {
            // If default attachments are enabled.
            if (_options.ShowDefaultAttachments)
            {
                var attachment = new Attachment
                {
                    Fallback = $"[{logEvent.Level}]{logEvent.RenderMessage()}",
                    Color = _options.AttachmentColors[logEvent.Level],
                    Fields = new List<Field>()
                };

                AddAttachmentField(ref attachment, new Field { Title = "Level", Value = logEvent.Level.ToString(), Short = _options.DefaultAttachmentsShortFormat });
                AddAttachmentField(ref attachment, new Field { Title = "Timestamp", Value = logEvent.Timestamp.ToString(_options.TimestampFormat), Short = _options.DefaultAttachmentsShortFormat });

                if (attachment.Fields.Any())
                    yield return attachment;
            }

            if (_options.ShowPropertyAttachments)
            {
                var fields = new List<Field>();

                using (var stringWriter = new StringWriter())
                {
                    foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
                    {
                        if (!_options.PropertyAllowList?.Any(x => x.Equals(property.Key, StringComparison.OrdinalIgnoreCase)) ?? false)
                            continue;
                        else if (_options.PropertyDenyList?.Any(x => x.Equals(property.Key, StringComparison.OrdinalIgnoreCase)) ?? false)
                            continue;

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
                }

                if (fields.Any())
                {
                    yield return new Attachment
                    {
                        Fallback = $"[{logEvent.Level}]{logEvent.RenderMessage()}",
                        Color = _options.AttachmentColors[logEvent.Level],
                        Fields = fields
                    };
                }
            }

            // If there is an exception in the current event,
            // and exception attachments are enabled.
            if (logEvent.Exception != null && _options.ShowExceptionAttachments)
            {
                var attachment = new Attachment
                {
                    Title = "Exception",
                    Fallback = $"Exception: {logEvent.Exception.Message} \n {ShortenMessage(logEvent.Exception.StackTrace, 1000)}",
                    Color = _options.AttachmentColors[LogEventLevel.Fatal],
                    Fields = new List<Field>(),
                    MrkdwnIn = new List<string> { "fields" }
                };

                AddAttachmentField(ref attachment, new Field { Title = "Message", Value = logEvent.Exception.Message });
                AddAttachmentField(ref attachment, new Field { Title = "Type", Value = $"`{logEvent.Exception.GetFlattenedType()}`" });

                AddAttachmentField(ref attachment, new Field { Title = "Exception", Value = $"```{ShortenMessage(logEvent.Exception.GetFlattenedMessage(), 1000)}```", Short = false });

                if (!string.IsNullOrEmpty(logEvent.Exception.StackTrace))
                    AddAttachmentField(ref attachment, new Field { Title = "Stack Trace", Value = $"```{ShortenMessage(logEvent.Exception.GetFlattenedStackTrace(), 1000)}```", Short = false });

                if (attachment.Fields.Any())
                    yield return attachment;
            }
        }

        private void AddAttachmentField(ref Attachment attachment, Field field)
        {
            if (!_options.PropertyAllowList?.Any(x => x.Equals(field.Title, StringComparison.OrdinalIgnoreCase)) ?? false)
                return;
            else if (_options.PropertyDenyList?.Any(x => x.Equals(field.Title, StringComparison.OrdinalIgnoreCase)) ?? false)
                return;

            attachment.Fields.Add(field);
        }

        private static string ShortenMessage(string message, int maxLength)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            if (message.Length < maxLength)
                return message;

            return message.Substring(0, maxLength - 3) + "...";
        }
    }
}