using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Slack.Enums;
using Serilog.Sinks.Slack.Helpers;
using Serilog.Sinks.Slack.Models;

namespace Serilog.Sinks.Slack
{
    public class SlackMessageFormatter
    {
        private readonly ITextFormatter _textFormatter;
        private readonly SlackSinkOptions _sinkOptions;

        public SlackMessageFormatter(ITextFormatter textFormatter, SlackSinkOptions sinkOptions)
        {
            _textFormatter = textFormatter;
            _sinkOptions = sinkOptions;
        }

        public Message CreateMessage(LogEvent logEvent)
        {
            using (var textWriter = new StringWriter())
            {
                _textFormatter.Format(logEvent, textWriter);

                return new Message
                {
                    Text = textWriter.ToString(),
                    Channel = GetPropertyFromLogEvent(logEvent, OverridableProperties.CustomChannel, _sinkOptions.CustomChannel),
                    UserName = GetPropertyFromLogEvent(logEvent, OverridableProperties.CustomUserName, _sinkOptions.CustomUserName),
                    IconEmoji = GetPropertyFromLogEvent(logEvent, OverridableProperties.CustomIcon, _sinkOptions.CustomIcon),
                    Attachments = CreateAttachments(logEvent).ToList()
                };
            }
        }

        protected IEnumerable<Attachment> CreateAttachments(LogEvent logEvent)
        {
            // If default attachments are enabled.
            if (_sinkOptions.ShowDefaultAttachments)
            {
                var attachment = new Attachment
                {
                    Fallback = $"[{logEvent.Level}]{logEvent.RenderMessage()}",
                    Color = _sinkOptions.AttachmentColors[logEvent.Level],
                    Fields = new List<Field>()
                };

                AddAttachmentField(ref attachment, new Field { Title = "Level", Value = logEvent.Level.ToString(), Short = _sinkOptions.DefaultAttachmentsShortFormat });
                AddAttachmentField(ref attachment, new Field { Title = "Timestamp", Value = logEvent.Timestamp.ToString(_sinkOptions.TimestampFormat), Short = _sinkOptions.DefaultAttachmentsShortFormat });

                if (attachment.Fields.Any())
                    yield return attachment;
            }

            if (_sinkOptions.ShowPropertyAttachments)
            {
                var fields = new List<Field>();

                using (var stringWriter = new StringWriter())
                {
                    foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
                    {
                        if (_sinkOptions.PropertyOverrideList?.Any(x => Enum.GetName(typeof(OverridableProperties), x).Equals(property.Key, StringComparison.OrdinalIgnoreCase)) ?? false)
                            continue;
                        else if (!_sinkOptions.PropertyAllowList?.Any(x => x.Equals(property.Key, StringComparison.OrdinalIgnoreCase)) ?? false)
                            continue;
                        else if (_sinkOptions.PropertyDenyList?.Any(x => x.Equals(property.Key, StringComparison.OrdinalIgnoreCase)) ?? false)
                            continue;

                        property.Value.Render(stringWriter);
                        var field = new Field
                        {
                            Title = property.Key,
                            Value = stringWriter.ToString(),
                            Short = _sinkOptions.PropertyAttachmentsShortFormat
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
                        Color = _sinkOptions.AttachmentColors[logEvent.Level],
                        Fields = fields
                    };
                }
            }

            // If there is an exception in the current event,
            // and exception attachments are enabled.
            if (logEvent.Exception != null && _sinkOptions.ShowExceptionAttachments)
            {
                var attachment = new Attachment
                {
                    Title = "Exception",
                    Fallback = $"Exception: {logEvent.Exception.Message} \n {ShortenMessage(logEvent.Exception.StackTrace, 1000)}",
                    Color = _sinkOptions.AttachmentColors[LogEventLevel.Fatal],
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
            if (!_sinkOptions.PropertyAllowList?.Any(x => x.Equals(field.Title, StringComparison.OrdinalIgnoreCase)) ?? false)
                return;
            else if (_sinkOptions.PropertyDenyList?.Any(x => x.Equals(field.Title, StringComparison.OrdinalIgnoreCase)) ?? false)
                return;

            attachment.Fields.Add(field);
        }

        private string GetPropertyFromLogEvent(LogEvent logEvent, OverridableProperties overridableProperty, string defaultValue)
        {
            if (!_sinkOptions.PropertyOverrideList?.Contains(overridableProperty) ?? true) return defaultValue;

            var overridablePropertyName = Enum.GetName(typeof(OverridableProperties), overridableProperty);
            if (!logEvent.Properties.TryGetValue(overridablePropertyName, out var value))
                return defaultValue;

            var stringValue = value is LogEventPropertyValue logEventPropertyValue ? logEventPropertyValue.ToString().Replace("\"", string.Empty) : defaultValue;
            return !string.IsNullOrEmpty(stringValue)
                ? stringValue
                : defaultValue;
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
