using System.IO;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Options
{
    public class ReportGeneratorOptions
    {
        public string Reports { get; set; } = null!;
        public string ReportTypes { get; set; } = null!;
        public string DestinationPath { get; set; } = "coverage-report";
        public string[]? ExtraArguments { get; set; }

        public static void Configure(ReportGeneratorOptions options,
            IEnvironmentVariableProvider environmentVariableProvider)
        {
            string? reportTypes = environmentVariableProvider.GetEnvironmentVariable("REPORT_TYPES");
            options.ReportTypes = reportTypes ?? "JsonSummary;Html";

            string? reports = environmentVariableProvider.GetEnvironmentVariable("REPORTS");
            options.Reports = reports ?? $"**{Path.DirectorySeparatorChar}coverage*.xml";

            string? extraArgsCountString = environmentVariableProvider.GetEnvironmentVariable("EXTRA_ARGS_COUNT");
            bool parsedExtraArgsCount = int.TryParse(extraArgsCountString, out int extraArgsCount);
            if (!parsedExtraArgsCount || extraArgsCount <= 0) {
                return;
            }

            options.ExtraArguments = new string[extraArgsCount];
            for (int i = 0; i < extraArgsCount; i++) {
                options.ExtraArguments[i] =
                    environmentVariableProvider.GetRequiredEnvironmentVariable($"EXTRA_ARGS_{i}");
            }
        }
    }
}
