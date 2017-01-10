# serilog-sinks-slack

<p align="center">
    <a href="https://ci.appveyor.com/project/mgibas/serilog-sinks-slack/branch/master">
        <img src="https://ci.appveyor.com/api/projects/status/hgfjns15mkqih2lx/branch/master?svg=true"></img>
    </a>
    <a href="https://www.gitcheese.com/app/#/projects/9d982e9c-315b-40ec-a580-3c6540e2700c/pledges/create">
        <img src="https://api.gitcheese.com/v1/projects/9d982e9c-315b-40ec-a580-3c6540e2700c/badges"></img>
    </a>
    <a href="https://www.nuget.org/packages/Serilog.Sinks.Slack/">
        <img src="https://img.shields.io/nuget/v/Serilog.Sinks.Slack.svg?style=flat-square"></img>
    </a>
</p>
<p align="center">
    <img src="https://img.shields.io/badge/.net-4.5-green.svg"></img>
    <img src="https://img.shields.io/badge/.net-4.6-green.svg"></img>
    <img src="https://img.shields.io/badge/.net%20standart-1.1-green.svg"></img>
    <img src="https://img.shields.io/badge/.net%20standart-1.3-green.svg"></img>
</p>

===

Simple and beautifull Serilog Slack sink :)

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
```

Custom channel, username or icon:
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Slack("https://hooks.slack.com/services/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",  20, TimeSpan.FromSeconds(10), "#general" ,"Im a Ghost", ":ghost:")
```
