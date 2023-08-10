using System;
using System.Collections.Generic;
using NSubstitute;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.Helpers;

public abstract class DefaultEnvironmentVariableProvider : IEnvironmentVariableProvider
{
    public virtual string GetEnvironmentVariable(string variableName) => throw new NotImplementedException();
}

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

        var envVarProvider = Substitute.ForPartsOf<DefaultEnvironmentVariableProvider>();
        envVarProvider.GetEnvironmentVariable(Arg.Any<string>())
            .Returns(x => environment.GetValueOrDefault(x.ArgAt<string>(0)));

        return envVarProvider;
    }
}
