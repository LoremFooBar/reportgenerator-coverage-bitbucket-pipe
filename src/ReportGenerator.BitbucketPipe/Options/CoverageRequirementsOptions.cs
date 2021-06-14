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
            string? lineCoverageString = environmentVariableProvider.GetEnvironmentVariable(EnvironmentVariable.LineCoverageMinimum);
            string? branchCoverageString = environmentVariableProvider.GetEnvironmentVariable(EnvironmentVariable.BranchCoverageMinimum);

            _ = int.TryParse(lineCoverageString, out int lineCoverageMinimum);
            _ = int.TryParse(branchCoverageString, out int branchCoverageMinimum);

            options.LineCoveragePercentageMinimum = lineCoverageMinimum;
            options.BranchCoveragePercentageMinimum = branchCoverageMinimum;
        }
    }
}
