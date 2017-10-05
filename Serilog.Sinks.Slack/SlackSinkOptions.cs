using Serilog.Events;
using System;
using System.Collections.Generic;

namespace Serilog.Sinks.Slack
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
    }
}
