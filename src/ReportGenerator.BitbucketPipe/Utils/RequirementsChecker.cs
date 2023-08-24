using ReportGenerator.BitbucketPipe.Model;
using ReportGenerator.BitbucketPipe.Options;

namespace ReportGenerator.BitbucketPipe.Utils;

public static class RequirementsChecker
{
    public static bool CoverageMeetsRequirements(CoverageRequirementsOptions requirementsOptions,
        CoverageSummary summary) =>
        requirementsOptions.BranchCoveragePercentageMinimum <= summary.BranchCoveragePercentage &&
        requirementsOptions.LineCoveragePercentageMinimum <= summary.LineCoveragePercentage;
}
