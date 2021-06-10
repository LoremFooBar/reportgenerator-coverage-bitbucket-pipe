using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly string _coverageReportPath;

        public CoverageReportGenerator(ILogger<CoverageReportGenerator> logger,
            IOptions<ReportGeneratorOptions> options, PipeEnvironment pipeEnvironment)
        {
            _logger = logger;
            _pipeEnvironment = pipeEnvironment;
            _options = options.Value;
            _coverageReportPath = "coverage-report";
        }

        public async Task<CoverageSummary> GenerateCoverageReportAsync()
        {
            RunCoverageReportGenerator();
            try {
                return await ParseCoverageSummaryAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error parsing summary report");
                return new CoverageSummary();
            }
        }

        private void RunCoverageReportGenerator()
        {
            string argumentsString = PrepareCommandArguments();

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var cancellationToken = cancellationTokenSource.Token;
            ProcessUtils.RunProcessUntilFinishedOrCanceled("reportgenerator", argumentsString,
                _logger, cancellationToken);
        }

        private string PrepareCommandArguments()
        {
            string verbosityLevel = _pipeEnvironment.IsDebugMode ? "Verbose" : "Warning";
            string[] basicArguments =
            {
                $"\"-reports:{_options.Reports}\"",
                $"-targetdir:{_coverageReportPath}",
                $"-reporttypes:{_options.ReportTypes}",
                $"-verbosity:{verbosityLevel}"
            };
            string[] allArguments;
            if (_options.ExtraArguments != null && _options.ExtraArguments.Length > 0) {
                ValidatePluginsArgument(_options.ExtraArguments);
                allArguments = basicArguments.Concat(_options.ExtraArguments).ToArray();
            }
            else {
                allArguments = basicArguments;
            }

            string argumentsString = string.Join(' ', allArguments);
            return argumentsString;
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
            await using var fileStream = File.OpenRead(Path.Combine(_coverageReportPath, "Summary.json"));

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
