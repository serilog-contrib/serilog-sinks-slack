using System;
using System.Collections.Generic;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using Serilog.Sinks.Slack.Enums;

namespace Serilog.Sinks.Slack.Models
{
    /// <summary>
    /// Container for all Slack sink configuration.
    /// </summary>
    public class SlackSinkOptions
    {
        /// <summary>
        /// Required: The incoming webhook URL from your Slack integrations page.
        /// </summary>
        public string WebHookUrl { get; set; }

        /// <summary>
        /// Show attachments for all logs without exceptions. Default is true.
        /// </summary>
        public bool ShowDefaultAttachments { get; set; } = true;

        /// <summary>
        ///  Use the short format for attachments of all logs without exceptions. Default is true.
        /// </summary>
        public bool DefaultAttachmentsShortFormat { get; set; } = true;

        /// <summary>
        /// Show properties from the log context in the attachments. Default is true.
        /// </summary>
        public bool ShowPropertyAttachments { get; set; } = true;

        /// <summary>
        /// Use the short format for properties from the log context in the attachments. Default is true.
        /// </summary>
        public bool PropertyAttachmentsShortFormat { get; set; } = true;

        /// <summary>
        /// Show attachments for exceptions, with the exception details. Default is true.
        /// </summary>
        public bool ShowExceptionAttachments { get; set; } = true;

        /// <summary>
        /// A mapping of log levels to slack message colours. Only applies when attachments are enabled.
        /// </summary>
        public IDictionary<LogEventLevel, string> AttachmentColors { get; } = new Dictionary<LogEventLevel, string>
        {
            {LogEventLevel.Verbose, "#777"},
            {LogEventLevel.Debug, "#777"},
            {LogEventLevel.Information, "#5bc0de"},
            {LogEventLevel.Warning, "#f0ad4e"},
            {LogEventLevel.Error, "#d9534f"},
            {LogEventLevel.Fatal, "#d9534f"}
        };

        /// <summary>
        /// Optional: How many messages to send to Slack at once. Defaults to 50.
        /// </summary>
        public int BatchSizeLimit { get; set; } = 50;

        /// <summary>
        /// Optional: The maximum period between message batches. Defaults to 5 seconds.
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Optional: A channel to post in. Default is whatever is set on the integration page.
        /// </summary>
        public string CustomChannel { get; set; }

        /// <summary>
        /// Optional: A user name to use when posting. Default is whatever is set on the integration page.
        /// </summary>
        public string CustomUserName { get; set; }

        /// <summary>
        /// Optional: A custom icon. Default is whatever is set on the integration page.
        /// </summary>
        public string CustomIcon { get; set; }

        /// <summary>
        /// Optional: A minimum log event level that will be sent to slack.
        /// </summary>
        public LogEventLevel MinimumLogEventLevel { get; set; }

        /// <summary>
        /// Optional: A list of properties (including exception properties) that are included in the messages.
        /// If this property is set along with <see cref="PropertyDenyList"/>, this takes precedence.
        /// </summary>
        public List<string> PropertyAllowList { get; set; }

        /// <summary>
        /// Optional: A list of properties (including exception properties) that are excluded in the messages.
        /// </summary>
        public List<string> PropertyDenyList { get; set; }

        /// <summary>
        /// Optional: A hashset of <see cref="SlackSinkOptions"> properties that are being overridden by properties in the messages.
        /// </summary>
        public HashSet<OverridableProperties> PropertyOverrideList { get; set; }

        /// <summary>
        /// Optional: The <see href="https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings"> date and time format</see> used for timestamps in the messages.
        /// Default is derived from the current culture. 
        /// </summary>
        public string TimestampFormat { get; set; }

        /// <summary>
        /// Maximum number of events to hold in the sink's internal queue, or <c>null</c>
        /// for an unbounded queue. The default is <c>100000</c>.
        /// </summary>
        public int? QueueLimit { get; set; } = 100000;

        /// <summary>
        /// Maps options to <see cref="PeriodicBatchingSinkOptions"/> for use with <see cref="PeriodicBatchingSink"/> ctor.
        /// </summary>
        /// <returns>Instance of <see cref="PeriodicBatchingSinkOptions"/> object.</returns>
        internal PeriodicBatchingSinkOptions ToPeriodicBatchingSinkOptions()
        {
            return new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = BatchSizeLimit,
                Period = Period,
                QueueLimit = QueueLimit
            };
        }
    }
}
