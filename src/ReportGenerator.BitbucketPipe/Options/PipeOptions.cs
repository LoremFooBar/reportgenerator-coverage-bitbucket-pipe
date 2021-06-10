using System;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Options
{
    public class PipeOptions
    {
        public bool CreateBuildStatus { get; set; }

        public static void Configure(PipeOptions options, IEnvironmentVariableProvider envVariableProvider)
        {
            options.CreateBuildStatus =
                envVariableProvider.GetEnvironmentVariableOrDefault("CREATE_BUILD_STATUS", "true")
                    .Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
