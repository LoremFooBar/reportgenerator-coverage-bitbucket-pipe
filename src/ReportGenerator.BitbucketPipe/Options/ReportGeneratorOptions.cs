using System;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Options
{
    public class ReportGeneratorOptions
    {
        public string Reports { get; set; } = null!;
        public string ReportTypes { get; set; } = null!;
        public string[]? ExtraArguments { get; set; }

        public static void Configure(ReportGeneratorOptions options,
            IEnvironmentVariableProvider environmentVariableProvider)
        {
            string? reportTypes = Environment.GetEnvironmentVariable("REPORT_TYPES");
            options.ReportTypes = reportTypes ?? "JsonSummary;Html";

            string? reports = Environment.GetEnvironmentVariable("REPORTS");
            options.Reports = reports ?? "**/coverage*.xml";

            string? extraArgsCountString = Environment.GetEnvironmentVariable("EXTRA_ARGS_COUNT");
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
