using System;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Slack;

namespace Serilog
{
    public static class SeqLoggerConfigurationExtensions
    {
        public static LoggerConfiguration Slack(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string webhookUrl,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerSinkConfiguration == null) throw new ArgumentNullException(nameof(loggerSinkConfiguration));
            if (string.IsNullOrWhiteSpace(webhookUrl)) throw new ArgumentNullException(nameof(webhookUrl));

            ILogEventSink sink = new SlackSink(webhookUrl);
            return loggerSinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
