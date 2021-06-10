using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ReportGenerator.BitbucketPipe.Utils;
using Serilog;

namespace ReportGenerator.BitbucketPipe
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        // ReSharper disable once InconsistentNaming
        private static async Task Main()
        {
            bool isDebugMode = new PipeEnvironment(new EnvironmentVariableProvider()).IsDebugMode;
            Log.Logger = LoggerInitializer.CreateLogger(isDebugMode);
            Log.Debug("DEBUG={IsDebug}", isDebugMode);
            Log.Debug("Workdir={Workdir}", Environment.CurrentDirectory);

            await new PipeRunner(new EnvironmentVariableProvider()).RunPipeAsync();
        }
    }
}
