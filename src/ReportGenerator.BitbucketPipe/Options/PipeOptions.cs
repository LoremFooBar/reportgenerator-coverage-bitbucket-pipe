using System;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Options;

[Serializable]
public class PipeOptions
{
    public bool CreateBuildStatus { get; set; }
    public bool FailWhenMinimumNotMet { get; set; }

    public static void Configure(PipeOptions options, IEnvironmentVariableProvider envVariableProvider)
    {
        options.CreateBuildStatus =
            envVariableProvider.GetEnvironmentVariableOrDefault("CREATE_BUILD_STATUS", "true")
                .Equals("true", StringComparison.OrdinalIgnoreCase);

        options.FailWhenMinimumNotMet =
            envVariableProvider.GetEnvironmentVariableOrDefault("FAIL_WHEN_MINIMUM_NOT_MET", "false")
                .Equals("true", StringComparison.OrdinalIgnoreCase);
    }
}
