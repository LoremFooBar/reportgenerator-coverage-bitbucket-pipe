using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ReportGenerator.BitbucketPipe;

[SuppressMessage("ReSharper", "ReplaceAutoPropertyWithComputedProperty")]
[DebuggerDisplay("{" + nameof(_name) + "}")]
public readonly struct EnvironmentVariable
{
    private EnvironmentVariable(string name) => _name = name;

    private readonly string _name;
    public static implicit operator string(EnvironmentVariable variable) => variable._name;
    public static implicit operator EnvironmentVariable(string name) => new(name);

    public static EnvironmentVariable BitbucketUsername { get; } = new("BITBUCKET_USERNAME");
    public static EnvironmentVariable BitbucketAppPassword { get; } = new("BITBUCKET_APP_PASSWORD");
    public static EnvironmentVariable LineCoverageMinimum { get; } = new("LINE_COVERAGE_MINIMUM");
    public static EnvironmentVariable BranchCoverageMinimum { get; } = new("BRANCH_COVERAGE_MINIMUM");
    public static EnvironmentVariable BuildStatusName { get; } = new("BUILD_STATUS_NAME");
    public static EnvironmentVariable CreateBuildStatus { get; } = new("CREATE_BUILD_STATUS");
    public static EnvironmentVariable PipelineReportTitle { get; } = new("PIPELINE_REPORT_TITLE");
    public static EnvironmentVariable PublishedReportUrl { get; } = new("PUBLISHED_REPORT_URL");
    public static EnvironmentVariable Reports { get; } = new("REPORTS");
    public static EnvironmentVariable ReportTypes { get; } = new("REPORT_TYPES");
    public static EnvironmentVariable ExtraArgs { get; } = new("EXTRA_ARGS");
    public static EnvironmentVariable ExtraArgsCount { get; } = new("EXTRA_ARGS_COUNT");
    public static EnvironmentVariable Debug { get; } = new("DEBUG");
    public static EnvironmentVariable NetCoreEnvironment { get; } = new("NETCORE_ENVIRONMENT");

    public static EnvironmentVariable BitbucketCommit { get; } = new("BITBUCKET_COMMIT");
    public static EnvironmentVariable BitbucketWorkspace { get; } = new("BITBUCKET_WORKSPACE");
    public static EnvironmentVariable BitbucketRepoSlug { get; } = new("BITBUCKET_REPO_SLUG");
}
