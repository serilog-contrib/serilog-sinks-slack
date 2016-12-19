using System;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Slack
{
    /// <summary>
    /// Provides extension methods on <see cref="LoggerSinkConfiguration"/>.
    /// </summary>
    public static class SlackLoggerConfigurationExtensions
    {
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
        /// <param name="batchSizeLimit">The time to wait between checking for event batches.</param>
        /// <param name="period">The time to wait between checking for event batches..</param>
        /// <param name="customChannel">Name of Slack channel to which message should be posted.</param>
        /// <param name="customUsername">User name that will be displayed as a name of the message sender.</param>
        /// <param name="customIcon">Icon that will be used as a sender avatar.</param>
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging level that will be allowed to be logged.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration Slack(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string webhookUrl,
            int? batchSizeLimit = null,
            TimeSpan? period = null,
            string customChannel = null,
            string customUsername = null,
            string customIcon = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            var slackSinkOptions = new SlackSinkOptions
            {
                WebHookUrl = webhookUrl,
                CustomChannel = customChannel,
                CustomUserName = customUsername,
                CustomIcon = customIcon
            };

            if (batchSizeLimit.HasValue)
            {
                slackSinkOptions.BatchSizeLimit = batchSizeLimit.Value;
            }

            if (period.HasValue)
            {
                slackSinkOptions.Period = period.Value;
            }

            return loggerSinkConfiguration.Slack(slackSinkOptions, restrictedToMinimumLevel);
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
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging level that will be allowed to be logged.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration Slack(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            SlackSinkOptions slackSinkOptions,
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

            if (string.IsNullOrWhiteSpace(slackSinkOptions.WebHookUrl))
            {
                throw new ArgumentNullException(nameof(slackSinkOptions.WebHookUrl));
            }

            return loggerSinkConfiguration.Sink(new SlackSink(slackSinkOptions), restrictedToMinimumLevel);
        }
    }
}