using Serilog.Sinks.Slack.Models;

namespace Serilog.Sinks.Slack.Enums
{
    /// <summary>
    /// These properties can be used to override options at a message level.
    /// Overrides can be enabled by adding their enum values to <see cref="SlackSinkOptions"> PropertyOverrideList property.
    /// </summary>
    public enum OverridableProperties
    {
        CustomChannel,
        CustomUserName,
        CustomIcon
    }
}
