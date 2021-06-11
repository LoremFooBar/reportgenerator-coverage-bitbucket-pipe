using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Palmmedia.ReportGenerator.Core;
using ReportGenerator.BitbucketPipe.Model;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe
{
    public class CoverageReportGenerator
    {
        private readonly ILogger<CoverageReportGenerator> _logger;
        private readonly PipeEnvironment _pipeEnvironment;
        private readonly ReportGeneratorOptions _options;

        public CoverageReportGenerator(ILogger<CoverageReportGenerator> logger,
            IOptions<ReportGeneratorOptions> options, PipeEnvironment pipeEnvironment)
        {
            _logger = logger;
            _pipeEnvironment = pipeEnvironment;
            _options = options.Value;
        }

        public async Task<CoverageSummary> GenerateCoverageReportAsync()
        {
            var cliArguments = PrepareCliArguments();
            var reportConfiguration = new ReportConfigurationBuilder().Create(cliArguments);
            bool reportGenerated = new Generator().GenerateReport(reportConfiguration);
            if (!reportGenerated) {
                throw new ReportGenerationFailedException();
            }

            return await ParseCoverageSummaryAsync();
        }

        private Dictionary<string, string> PrepareCliArguments()
        {
            string verbosityLevel = _pipeEnvironment.IsDebugMode ? "Verbose" : "Warning";
            Dictionary<string, string> basicArguments = new(StringComparer.OrdinalIgnoreCase)
            {
                ["reports"] = _options.Reports,
                ["targetdir"] = _options.DestinationPath,
                ["reporttypes"] = _options.ReportTypes,
                ["verbosity"] = verbosityLevel
            };

            Dictionary<string, string> allArguments;
            if (_options.ExtraArguments is {Length: > 0}) {
                ValidatePluginsArgument(_options.ExtraArguments);
                var extraArguments = CreateArgumentsDictionary(_options.ExtraArguments);
                allArguments = basicArguments.Concat(extraArguments.Where(kv => !basicArguments.ContainsKey(kv.Key)))
                    .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            }
            else {
                allArguments = basicArguments;
            }

            return allArguments;
        }

        private static Dictionary<string, string> CreateArgumentsDictionary(IEnumerable<string>? args)
        {
            if (args is null) {
                return new Dictionary<string, string>();
            }

            var res = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string extraArgument in args) {
                string[] splits = extraArgument.Split(':',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length != 2) {
                    continue;
                }

                string key = splits[0].TrimStart('"').TrimStart('-');
                string value = splits[1].TrimEnd('"');
                res.Add(key, value);
            }

            return res;
        }

        private void ValidatePluginsArgument(IEnumerable<string> optionsExtraArguments)
        {
            string? pluginsArg = optionsExtraArguments.FirstOrDefault(a => a.StartsWith("-plugins:"));
            if (pluginsArg == null) {
                return;
            }

            _logger.LogDebug("Found plugins in extra_args");
            string[] pluginsPaths = pluginsArg.Split(':')[1].Split(';');
            foreach (string path in pluginsPaths) {
                var fileInfo = new FileInfo(path);
                _logger.LogDebug("Checking existence of {Path}", path);
                bool exists = fileInfo.Exists;
                if (exists) {
                    _logger.LogDebug("{Path} exists", path);
                }
                else {
                    _logger.LogWarning("{Path} doesn't exist!", path);
                }

                if (!Path.IsPathFullyQualified(path)) {
                    _logger.LogWarning("The plugin at {Path} will probably fail to load, " +
                                       "because Report Generator will only load plugins using absolute paths", path);
                }
            }
        }

        private async Task<CoverageSummary> ParseCoverageSummaryAsync()
        {
            await using var fileStream = File.OpenRead(Path.Combine(_options.DestinationPath, "Summary.json"));

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var cancellationToken = cancellationTokenSource.Token;

            using var jsonDocument = await JsonDocument.ParseAsync(fileStream, cancellationToken: cancellationToken);
            var summaryElement = jsonDocument.RootElement.GetProperty("summary");
            var summaryEnumerator = summaryElement.EnumerateObject();
            var coverageSummary = new CoverageSummary
            {
                // ReSharper disable  StringLiteralTypo
                LineCoveragePercentage = summaryEnumerator.First(_ => _.NameEquals("linecoverage")).Value.GetDouble(),
                BranchCoveragePercentage =
                    summaryEnumerator.First(_ => _.NameEquals("branchcoverage")).Value.GetDouble()
                // ReSharper restore  StringLiteralTypo
            };

            return coverageSummary;
        }
    }
}
