using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ReportGenerator.BitbucketPipe.Utils;
using Serilog;
using static ReportGenerator.BitbucketPipe.Utils.EnvironmentUtils;

namespace ReportGenerator.BitbucketPipe
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        // ReSharper disable once InconsistentNaming
        private static async Task Main()
        {
            Log.Logger = LoggerInitializer.CreateLogger(IsDebugMode);

            Log.Debug("DEBUG={IsDebug}", IsDebugMode);
            Log.Debug("Workdir={Workdir}", Environment.CurrentDirectory);

            await new PipeRunner(new EnvironmentVariableProvider()).RunPipeAsync();
        }
    }
}
