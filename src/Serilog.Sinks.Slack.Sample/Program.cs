using System;
using Serilog.Events;

namespace Serilog.Sinks.Slack.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Slack("https://hooks.slack.com/services/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX")
                .CreateLogger();

            Log.Logger.Verbose("1 Verbose");
            Log.Logger.Debug("2 Debug");
            Log.Logger.Error("3 Error");
            try
            {
                throw new Exception("some logged exception!");
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "4 Fatal");
            }
            Log.Logger.Information("5 Information");
            Log.Logger.Warning("6 Warning");
            Log.Logger.Debug("7 Formatting {myProp}", new { myProp = "test" });
        }
    }
}
