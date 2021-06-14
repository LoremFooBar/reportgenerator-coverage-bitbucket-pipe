namespace ReportGenerator.BitbucketPipe.Utils
{
    public class BitbucketEnvironmentInfo
    {
        public BitbucketEnvironmentInfo(IEnvironmentVariableProvider? environmentVariableProvider = null)
        {
            if (environmentVariableProvider == null) {
                return;
            }

            CommitHash = environmentVariableProvider.GetRequiredEnvironmentVariable("BITBUCKET_COMMIT");
            Workspace = environmentVariableProvider.GetRequiredEnvironmentVariable("BITBUCKET_WORKSPACE");
            RepoSlug = environmentVariableProvider.GetRequiredEnvironmentVariable("BITBUCKET_REPO_SLUG");
        }

        public string Workspace { get; init; } = "";
        public string RepoSlug { get; init; } = "";
        public string CommitHash { get; init; } = "";
    }
}
