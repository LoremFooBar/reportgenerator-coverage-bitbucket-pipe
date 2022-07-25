using System.Collections.Generic;
using Moq;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.Helpers;

public static class TestEnvironment
{
    public static BitbucketEnvironmentInfo EnvironmentInfo { get; } = new()
    {
        Workspace = "workspace",
        RepoSlug = "repo-slug",
        CommitHash = "222be690",
    };

    public static IEnvironmentVariableProvider CreateMockEnvironment(
        Dictionary<EnvironmentVariable, string> environment, bool addBitbucketEnvironmentVariables = true)
    {
        if (addBitbucketEnvironmentVariables) {
            environment.TryAdd(EnvironmentVariable.BitbucketCommit, "222be690");
            environment.TryAdd(EnvironmentVariable.BitbucketWorkspace, "workspace");
            environment.TryAdd(EnvironmentVariable.BitbucketRepoSlug, "repo-slug");
        }

        var envVarMock = new Mock<IEnvironmentVariableProvider> { CallBase = true };
        envVarMock.Setup(provider => provider.GetEnvironmentVariable(It.IsAny<string>()))
            .Returns((string varName) => environment.GetValueOrDefault(varName));

        return envVarMock.Object;
    }
}
