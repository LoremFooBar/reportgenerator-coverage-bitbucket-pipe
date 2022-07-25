using System.IO;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Options;

public class ReportGeneratorOptions
{
    public const string DefaultDestinationPath = "coverage-report";

    public string Reports { get; set; } = null!;
    public string ReportTypes { get; set; } = null!;
    public string DestinationPath { get; set; } = DefaultDestinationPath;
    public string[]? ExtraArguments { get; set; }

    public static void Configure(ReportGeneratorOptions options,
        IEnvironmentVariableProvider environmentVariableProvider)
    {
        string? reportTypes = environmentVariableProvider.GetEnvironmentVariable(EnvironmentVariable.ReportTypes);
        options.ReportTypes = reportTypes ?? "JsonSummary;Html";

        string? reports = environmentVariableProvider.GetEnvironmentVariable(EnvironmentVariable.Reports);
        options.Reports = reports ?? $"**{Path.DirectorySeparatorChar}coverage*.xml";

        string? extraArgsCountString =
            environmentVariableProvider.GetEnvironmentVariable(EnvironmentVariable.ExtraArgsCount);
        bool parsedExtraArgsCount = int.TryParse(extraArgsCountString, out int extraArgsCount);

        if (!parsedExtraArgsCount || extraArgsCount <= 0) return;

        options.ExtraArguments = new string[extraArgsCount];
        string extraArgsPrefix = EnvironmentVariable.ExtraArgs;

        for (int i = 0; i < extraArgsCount; i++) {
            options.ExtraArguments[i] =
                environmentVariableProvider.GetRequiredEnvironmentVariable($"{extraArgsPrefix}_{i}");
        }
    }
}
