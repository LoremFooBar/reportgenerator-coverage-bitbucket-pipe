using System;

namespace ReportGenerator.BitbucketPipe.Utils;

public class PipeEnvironment(IEnvironmentVariableProvider environmentVariableProvider)
{
    public bool IsDebugMode { get; } = environmentVariableProvider
        .GetEnvironmentVariableOrDefault(EnvironmentVariable.Debug, "false")
        .Equals("true", StringComparison.OrdinalIgnoreCase);
}
