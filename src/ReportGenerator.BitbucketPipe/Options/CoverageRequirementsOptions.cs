using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Options
{
    public class CoverageRequirementsOptions
    {
        public int LineCoveragePercentageMinimum { get; set; }
        public int BranchCoveragePercentageMinimum { get; set; }

        public static void Configure(CoverageRequirementsOptions options, IEnvironmentVariableProvider
            environmentVariableProvider)
        {
            string? lineCoverageString = environmentVariableProvider.GetEnvironmentVariable("LINE_COVERAGE_MINIMUM");
            string? branchCoverageString = environmentVariableProvider.GetEnvironmentVariable("BRANCH_COVERAGE_MINIMUM");

            _ = int.TryParse(lineCoverageString, out int lineCoverageMinimum);
            _ = int.TryParse(branchCoverageString, out int branchCoverageMinimum);

            options.LineCoveragePercentageMinimum = lineCoverageMinimum;
            options.BranchCoveragePercentageMinimum = branchCoverageMinimum;
        }
    }
}
