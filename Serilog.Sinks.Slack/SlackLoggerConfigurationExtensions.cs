using System;
using System.Collections.Generic;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.Slack.Models;

namespace Serilog.Sinks.Slack
{
    /// <summary>
    /// Provides extension methods on <see cref="LoggerSinkConfiguration"/>.
    /// </summary>
    public static class SlackLoggerConfigurationExtensions
    {
        private const string DefaultOutputTemplate = "{Message}";

        /// <summary>
        /// <see cref="SlackLoggerConfigurationExtensions"/> extension that provides configuration chaining.
        /// <example>
        ///     new LoggerConfiguration()
        ///         .MinimumLevel.Verbose()
        ///         .WriteTo.Slack("webHookUrl", "channel" ,"username", "icon")
        ///         .CreateLogger();
        /// </example>
        /// </summary>
        /// <param name="loggerSinkConfiguration">Instance of <see cref="LoggerSinkConfiguration"/> object.</param>
        /// <param name="webhookUrl">Slack team post URI.</param>
        /// <param name="batchSizeLimit">The maximum number of events to include in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="customChannel">Name of Slack channel to which message should be posted.</param>
        /// <param name="customUsername">User name that will be displayed as a name of the message sender.</param>
        /// <param name="customIcon">Icon that will be used as a sender avatar.</param>
        /// <param name="showDefaultAttachments">Show attachments for all logs without exceptions. Default is true.</param>
        /// <param name="defaultAttachmentsShortFormat">Use the short format for attachments of all logs without exceptions. Default is true.</param>
        /// <param name="showPropertyAttachments">Show properties from the context in the attachments. Default is true.</param>
        /// <param name="propertyAttachmentsShortFormat">Use the short format for properties from the log context in the attachments. Default is true.</param>
        /// <param name="showExceptionAttachments">Show attachments containing exception details. Default is true.</param>
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging level that will be allowed to be logged.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink. The default is "{Message}".</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="propertyAllowList">If specified, only properties that match (case-insensitive) the name in the list are logged. Takes precedence over <see cref="SlackSinkOptions.PropertyDenyList"/></param>
        /// <param name="propertyDenyList">If specified, only properties that are not in this list are logged.</param>
        /// <param name="timestampFormat">The <see href="https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings"> date and time format</see> for timestamps in messages.</param>
        /// <param name="queueLimit">The maximum number of events to hold in the sink's internal queue, or <c>null</c> for an unbounded queue. The default is <c>100000</c>.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration Slack(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string webhookUrl,
            int? batchSizeLimit = null,
            TimeSpan? period = null,
            string customChannel = null,
            string customUsername = null,
            string customIcon = null,
            bool showDefaultAttachments = true,
            bool defaultAttachmentsShortFormat = true,
            bool showPropertyAttachments = true,
            bool propertyAttachmentsShortFormat = true,
            bool showExceptionAttachments = true,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            List<string> propertyAllowList = null,
            List<string> propertyDenyList = null,
            string timestampFormat = null,
            int? queueLimit = 100000
#if NETSTANDARD2_0_OR_GREATER || NETFRAMEWORK
            ,
            string proxyAddress = null,
            string proxyUsername = null,
            string proxyPassword = null
#endif
            )
        {

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return loggerSinkConfiguration.Slack(
                webhookUrl,
                formatter,
                batchSizeLimit,
                period,
                customChannel,
                customUsername,
                customIcon,
                showDefaultAttachments,
                defaultAttachmentsShortFormat,
                showPropertyAttachments,
                propertyAttachmentsShortFormat,
                showExceptionAttachments,
                restrictedToMinimumLevel,
                propertyAllowList,
                propertyDenyList,
                timestampFormat,
                queueLimit
#if NETSTANDARD2_0_OR_GREATER || NETFRAMEWORK
                ,
                proxyAddress,
                proxyUsername,
                proxyPassword
#endif
                );
        }

        /// <summary>
        /// <see cref="SlackLoggerConfigurationExtensions"/> extension that provides configuration chaining.
        /// <example>
        ///     new LoggerConfiguration()
        ///         .MinimumLevel.Verbose()
        ///         .WriteTo.Slack("webHookUrl", formatter, "channel" ,"username", "icon")
        ///         .CreateLogger();
        /// </example>
        /// </summary>
        /// <param name="loggerSinkConfiguration">Instance of <see cref="LoggerSinkConfiguration"/> object.</param>
        /// <param name="webhookUrl">Slack team post URI.</param>
        /// <param name="formatter">A formatter, such as <see cref="MessageTemplateTextFormatter"/>, to convert the log 
        /// events into text for Slack. If control of regular text formatting is required, use the other overload of 
        /// <see cref="SlackSink"/> and specify the outputTemplate parameter instead.
        /// </param>
        /// <param name="batchSizeLimit">The maximum number of events to include in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="customChannel">Name of Slack channel to which message should be posted.</param>
        /// <param name="customUsername">User name that will be displayed as a name of the message sender.</param>
        /// <param name="customIcon">Icon that will be used as a sender avatar.</param>
        /// <param name="showDefaultAttachments">Show attachments for all logs without exceptions. Default is true.</param>
        /// <param name="defaultAttachmentsShortFormat">Use the short format for attachments of all logs without exceptions. Default is true.</param>
        /// <param name="showPropertyAttachments">Show properties from the context in the attachments. Default is true.</param>
        /// <param name="propertyAttachmentsShortFormat">Use the short format for properties from the log context in the attachments. Default is true.</param>
        /// <param name="showExceptionAttachments">Show attachments containing exception details. Default is true.</param>
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging level that will be allowed to be logged.</param>
        /// <param name="propertyAllowList">If specified, only properties that match (case-insensitive) the name in the list are logged. Takes precedence over <see cref="SlackSinkOptions.PropertyDenyList"/></param>
        /// <param name="propertyDenyList">If specified, only properties that are not in this list are logged.</param>
        /// <param name="timestampFormat">The <see href="https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings"> date and time format</see> for timestamps in messages.</param>
        /// <param name="queueLimit">The maximum number of events to hold in the sink's internal queue, or <c>null</c> for an unbounded queue. The default is <c>100000</c>.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration Slack(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string webhookUrl,
            ITextFormatter formatter,
            int? batchSizeLimit = null,
            TimeSpan? period = null,
            string customChannel = null,
            string customUsername = null,
            string customIcon = null,
            bool showDefaultAttachments = true,
            bool defaultAttachmentsShortFormat = true,
            bool showPropertyAttachments = true,
            bool propertyAttachmentsShortFormat = true,
            bool showExceptionAttachments = true,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            List<string> propertyAllowList = null,
            List<string> propertyDenyList = null,
            string timestampFormat = null,
            int? queueLimit = 100000
#if NETSTANDARD2_0_OR_GREATER || NETFRAMEWORK
            ,
            string proxyAddress = null,
            string proxyUsername = null,
            string proxyPassword = null
#endif
            )
        {
            var slackSinkOptions = new SlackSinkOptions
            {
                WebHookUrl = webhookUrl,
                CustomChannel = customChannel,
                CustomUserName = customUsername,
                CustomIcon = customIcon,
                ShowDefaultAttachments = showDefaultAttachments,
                DefaultAttachmentsShortFormat = defaultAttachmentsShortFormat,
                ShowPropertyAttachments = showPropertyAttachments,
                PropertyAttachmentsShortFormat = propertyAttachmentsShortFormat,
                PropertyAllowList = propertyAllowList,
                PropertyDenyList = propertyDenyList,
                ShowExceptionAttachments = showExceptionAttachments,
                TimestampFormat = timestampFormat,
                QueueLimit = queueLimit,

            };


#if NETSTANDARD2_0_OR_GREATER || NETFRAMEWORK
            if(proxyAddress != null)
            {
                slackSinkOptions.ProxyAddress = Uri.IsWellFormedUriString(proxyAddress, UriKind.Absolute) 
                    ? new Uri(proxyAddress) : throw new ArgumentException("Proxy address invalid", nameof(proxyAddress));
                slackSinkOptions.ProxyUsername = proxyUsername ?? "";
                slackSinkOptions.ProxyPassword = proxyPassword ?? "";
            }
#endif


            if (batchSizeLimit.HasValue)
            {
                slackSinkOptions.BatchSizeLimit = batchSizeLimit.Value;
            }

            if (period.HasValue)
            {
                slackSinkOptions.Period = period.Value;
            }

            return loggerSinkConfiguration.Slack(slackSinkOptions, formatter, restrictedToMinimumLevel);
        }

        /// <summary>
        /// <see cref="LoggerSinkConfiguration"/> extension that provides configuration chaining.
        /// <example>
        ///     new LoggerConfiguration()
        ///         .MinimumLevel.Verbose()
        ///         .WriteTo.Slack("webHookUrl", "channel" ,"username", "icon")
        ///         .CreateLogger();
        /// </example>
        /// </summary>
        /// <param name="loggerSinkConfiguration">Instance of <see cref="LoggerSinkConfiguration"/> object.</param>
        /// <param name="slackSinkOptions">The slack sink options object.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink. The default is "{Message}".</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging level that will be allowed to be logged.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration Slack(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            SlackSinkOptions slackSinkOptions,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return loggerSinkConfiguration.Slack(slackSinkOptions, formatter, restrictedToMinimumLevel);
        }

        /// <summary>
        /// <see cref="LoggerSinkConfiguration"/> extension that provides configuration chaining.
        /// <example>
        ///     new LoggerConfiguration()
        ///         .MinimumLevel.Verbose()
        ///         .WriteTo.Slack("webHookUrl", formatter, "channel" ,"username", "icon")
        ///         .CreateLogger();
        /// </example>
        /// </summary>
        /// <param name="loggerSinkConfiguration">Instance of <see cref="LoggerSinkConfiguration"/> object.</param>
        /// <param name="slackSinkOptions">The slack sink options object.</param>
        /// <param name="formatter">A formatter, such as <see cref="MessageTemplateTextFormatter"/>, to convert the log events into
        /// text for Slack. If control of regular text formatting is required, use the other
        /// overload of <see cref="SlackSink"/> and specify the outputTemplate parameter instead.
        /// </param>
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging level that will be allowed to be logged.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration Slack(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            SlackSinkOptions slackSinkOptions,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerSinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerSinkConfiguration));
            }

            if (slackSinkOptions == null)
            {
                throw new ArgumentNullException(nameof(slackSinkOptions));
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            if (string.IsNullOrWhiteSpace(slackSinkOptions.WebHookUrl))
            {
                throw new ArgumentNullException(nameof(slackSinkOptions.WebHookUrl));
            }

            return loggerSinkConfiguration.Sink(new SlackSink(slackSinkOptions, formatter), restrictedToMinimumLevel);
        }
    }
}