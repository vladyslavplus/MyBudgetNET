using Serilog;
using Serilog.Events;

namespace MyBudget.API.Logging;

public static class SerilogConfiguration
{
    public static void ConfigureSerilog(this IHostBuilder host)
    {
        host.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig
                .WriteTo.Console()
                .WriteTo.File(
                    path: "Logs/volunplatform-.log",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                );
        });
    }
}