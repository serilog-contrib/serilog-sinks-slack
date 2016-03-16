# [![gitcheese.com](https://api.gitcheese.com/v1/projects/9d982e9c-315b-40ec-a580-3c6540e2700c/badges?size=s)](https://www.gitcheese.com/app/#/projects/9d982e9c-315b-40ec-a580-3c6540e2700c/pledges/create) serilog-sinks-slack [![Build status](https://ci.appveyor.com/api/projects/status/hgfjns15mkqih2lx?svg=true)](https://ci.appveyor.com/project/mgibas/serilog-sinks-slack)

====
Serilog Slack sink

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
    .WriteTo.Slack("https://hooks.slack.com/services/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX","#general" ,"Im a Ghost", ":ghost:")
```

===
[![gitcheese.com](https://api.gitcheese.com/v1/projects/9d982e9c-315b-40ec-a580-3c6540e2700c/badges?size=s)](https://www.gitcheese.com/app/#/projects/9d982e9c-315b-40ec-a580-3c6540e2700c/pledges/create)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  [![gitcheese.com](https://api.gitcheese.com/v1/projects/9d982e9c-315b-40ec-a580-3c6540e2700c/badges?size=s)](https://www.gitcheese.com/app/#/projects/9d982e9c-315b-40ec-a580-3c6540e2700c/pledges/create)

