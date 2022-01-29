# serilog-sinks-slack

<p align="center">
    <a href="https://ci.appveyor.com/project/mgibas/serilog-sinks-slack/branch/master">
        <img src="https://ci.appveyor.com/api/projects/status/hgfjns15mkqih2lx/branch/master?svg=true"></img>
    </a>
    <a href="https://www.nuget.org/packages/Serilog.Sinks.Slack/">
        <img src="https://img.shields.io/nuget/v/Serilog.Sinks.Slack.svg?style=flat-square"></img>
    </a>
</p>
<p align="center">
    <img src="https://img.shields.io/badge/.net-4.5-green.svg"></img>
    <img src="https://img.shields.io/badge/.net-4.6-green.svg"></img>
    <img src="https://img.shields.io/badge/.net%20standard-1.1-green.svg"></img>
    <img src="https://img.shields.io/badge/.net%20standard-1.3-green.svg"></img>
	<img src="https://img.shields.io/badge/.net%20standard-2.0-green.svg"></img>
</p>

Simple and beautiful Serilog Slack sink :)

NuGet
====
```
Install-Package Serilog.Sinks.Slack
```
Sample
====
![Sample](/example.png?raw=true "Slack Sample")

Usages
====

Minimal (using default WebHook integration settings)
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Slack("https://hooks.slack.com/services/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX")
    .CreateLogger();
```

Custom channel, username or icon:
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Slack("https://hooks.slack.com/services/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", 20, 1000, TimeSpan.FromSeconds(10), "#general" ,"Im a Ghost", ":ghost:")
    .CreateLogger();
```

Advanced:
```csharp
 Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Slack(new SlackSinkOptions()
        {
            WebHookUrl = "https://hooks.slack.com/services/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            CustomChannel = "#test",
            BatchSizeLimit = 20,
            QueueLimit = 1000,
            CustomIcon = ":ghost:",
            Period = TimeSpan.FromSeconds(10),
            ShowDefaultAttachments = false,
            ShowExceptionAttachments = true,
        })
        .CreateLogger();
```
