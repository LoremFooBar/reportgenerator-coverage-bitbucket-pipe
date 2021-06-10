using System.Diagnostics.CodeAnalysis;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ReportGenerator.BitbucketPipe
{
    [ExcludeFromCodeCoverage]
    public static class LoggerInitializer
    {
        public static Logger CreateLogger(bool isDebugOn)
        {
            var loggerConfig = new LoggerConfiguration().WriteTo.Console();
            loggerConfig.MinimumLevel.Is(isDebugOn ? LogEventLevel.Debug : LogEventLevel.Warning);
            return loggerConfig.CreateLogger();
        }
    }
}
