using System;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Options;

[Serializable]
public class PublishReportOptions
{
    public Uri? ReportUrl { get; set; }

    public static void Configure(PublishReportOptions options, IEnvironmentVariableProvider envVariableProvider)
    {
        string? reportUrlStr = envVariableProvider.GetEnvironmentVariable(EnvironmentVariable.PublishedReportUrl);
        Uri.TryCreate(reportUrlStr, UriKind.Absolute, out var reportUrl);
        options.ReportUrl = reportUrl;
    }
}
