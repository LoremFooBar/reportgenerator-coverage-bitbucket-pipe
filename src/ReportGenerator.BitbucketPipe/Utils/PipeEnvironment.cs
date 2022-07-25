using System;

namespace ReportGenerator.BitbucketPipe.Utils;

public class PipeEnvironment
{
    public PipeEnvironment(IEnvironmentVariableProvider environmentVariableProvider)
    {
        IsDebugMode = environmentVariableProvider
            .GetEnvironmentVariableOrDefault(EnvironmentVariable.Debug, "false")
            .Equals("true", StringComparison.OrdinalIgnoreCase);
        string environmentName =
            environmentVariableProvider.GetEnvironmentVariableOrDefault(EnvironmentVariable.NetCoreEnvironment,
                "Production");
        IsDevelopment = environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsDebugMode { get; }
    public bool IsDevelopment { get; }
}
