using System;

namespace ReportGenerator.BitbucketPipe.Utils
{
    public class PipeEnvironment
    {
        public PipeEnvironment(IEnvironmentVariableProvider environmentVariableProvider)
        {
            IsDebugMode = environmentVariableProvider.GetEnvironmentVariableOrDefault("DEBUG", "false")
                .Equals("true", StringComparison.OrdinalIgnoreCase);
            EnvironmentName =
                environmentVariableProvider.GetEnvironmentVariableOrDefault("NETCORE_ENVIRONMENT", "Production");
            IsDevelopment = EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsDebugMode { get; }

        public string EnvironmentName { get; }

        public bool IsDevelopment { get; }
    }
}
