using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportGenerator.BitbucketPipe.Model;
using ReportGenerator.BitbucketPipe.Model.Bitbucket.CommitStatuses;
using ReportGenerator.BitbucketPipe.Model.Bitbucket.Report;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe;

public class BitbucketClient
{
    private readonly BitbucketAuthenticationOptions _authOptions;
    private readonly BitbucketOptions _bitbucketOptions;
    private readonly BitbucketEnvironmentInfo _environmentInfo;
    private readonly HttpClient _httpClient;
    private readonly ILogger<BitbucketClient> _logger;
    private readonly PipeOptions _pipeOptions;
    private readonly PublishReportOptions _publishOptions;
    private readonly CoverageRequirementsOptions _requirementsOptions;

    public BitbucketClient(HttpClient client, ILogger<BitbucketClient> logger,
        IOptions<PublishReportOptions> publishOptions, IOptions<CoverageRequirementsOptions> requirementsOptions,
        IOptions<BitbucketOptions> bitbucketOptions, IOptions<PipeOptions> pipeOptions,
        IOptions<BitbucketAuthenticationOptions> authOptions, BitbucketEnvironmentInfo environmentInfo)
    {
        _httpClient = client;
        _logger = logger;
        _authOptions = authOptions.Value;
        _pipeOptions = pipeOptions.Value;
        _environmentInfo = environmentInfo;
        _bitbucketOptions = bitbucketOptions.Value;
        _publishOptions = publishOptions.Value;
        _requirementsOptions = requirementsOptions.Value;

        // when using the proxy in an actual pipelines environment, requests must be sent over http

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        client.BaseAddress =
            new Uri(
                $"https://api.bitbucket.org/2.0/repositories/{Workspace}/{RepoSlug}/commit/{CommitHash}/");

        _logger.LogDebug("Base address: {BaseAddress}", client.BaseAddress);
    }

    private string Workspace => _environmentInfo.Workspace;
    private string RepoSlug => _environmentInfo.RepoSlug;
    private string CommitHash => _environmentInfo.CommitHash;

    public async Task CreateCommitBuildStatusAsync(CoverageSummary summary)
    {
        if (!_pipeOptions.CreateBuildStatus) return;

        if (!_authOptions.UseAuthentication) {
            _logger.LogWarning("Will not create build status because authentication info was not provided");

            return;
        }

        _logger.LogDebug("Coverage requirements: {@CoverageRequirements}", _requirementsOptions);
        _logger.LogDebug("Coverage summary: {@CoverageSummary}", summary);

        bool meetsRequirements = RequirementsChecker.CoverageMeetsRequirements(_requirementsOptions, summary);

        _logger.LogDebug("Coverage meets requirements? {MeetsRequirements}", meetsRequirements);

        const string key = "Code-Coverage";
        var state = meetsRequirements ? State.Successful : State.Failed;
        var buildStatus = new BuildStatus(key, _bitbucketOptions.BuildStatusName, state, Workspace, RepoSlug,
                _publishOptions.ReportUrl)
            { Description = state == State.Failed ? "Coverage doesn't meet requirements" : "" };

        string serializedBuildStatus = Serialize(buildStatus);

        _logger.LogDebug("POSTing build status: {BuildStatus}", serializedBuildStatus);

        var response = await _httpClient.PostAsync("statuses/build", CreateStringContent(serializedBuildStatus));
        await VerifyResponseAsync(response);
    }

    public async Task CreateReportAsync(CoverageSummary summary)
    {
        var pipelineReport = new PipelineReport
        {
            Title = _bitbucketOptions.ReportTitle,
            Details = "Line and branch coverage summary",
            Link = _publishOptions.ReportUrl,
            ExternalId = "code-coverage",
            ReportType = ReportType.Coverage,
            Result = RequirementsChecker.CoverageMeetsRequirements(_requirementsOptions, summary)
                ? Result.Passed
                : Result.Failed,
            Data =
            {
                new ReportDataItem
                {
                    Title = "Line Coverage", Type = ReportDataType.Percentage,
                    Value = summary.LineCoveragePercentage,
                },
                new ReportDataItem
                {
                    Title = "Branch Coverage", Type = ReportDataType.Percentage,
                    Value = summary.BranchCoveragePercentage,
                },
            },
        };

        string serializedReport = Serialize(pipelineReport);

        _logger.LogDebug("PUTing report: {Report}", serializedReport);

        var response = await _httpClient.PutAsync($"reports/{pipelineReport.ExternalId}",
            CreateStringContent(serializedReport));
        await VerifyResponseAsync(response);
    }

    private static string Serialize(object obj)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        return JsonSerializer.Serialize(obj, jsonSerializerOptions);
    }

    private static StringContent CreateStringContent(string str) => new(str, Encoding.Default, "application/json");

    private async Task VerifyResponseAsync(HttpResponseMessage response)
    {
        _logger.LogDebug("Response status code: {StatusCode}", (int)response.StatusCode);

        if (!response.IsSuccessStatusCode) {
            string error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Error response: {Error}", error);
        }

        response.EnsureSuccessStatusCode();
    }
}
