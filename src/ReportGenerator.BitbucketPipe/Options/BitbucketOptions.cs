using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Options
{
    public class BitbucketOptions
    {
        public string ReportTitle { get; set; } = null!;
        public string BuildStatusName { get; set; } = null!;

        public static void Configure(BitbucketOptions options, IEnvironmentVariableProvider environmentVariableProvider)
        {
            string? reportTitle = environmentVariableProvider.GetEnvironmentVariable("PIPELINE_REPORT_TITLE");
            string? buildStatusName = environmentVariableProvider.GetEnvironmentVariable("BUILD_STATUS_NAME");

            if (string.IsNullOrWhiteSpace(reportTitle) && !string.IsNullOrWhiteSpace(buildStatusName)) {
                reportTitle = buildStatusName;
            }
            else if (string.IsNullOrWhiteSpace(buildStatusName) && !string.IsNullOrWhiteSpace(reportTitle)) {
                buildStatusName = reportTitle;
            }

            const string defaultTitle = "Code Coverage";
            options.BuildStatusName = buildStatusName ?? defaultTitle;
            options.ReportTitle = reportTitle ?? defaultTitle;
        }
    }
}
