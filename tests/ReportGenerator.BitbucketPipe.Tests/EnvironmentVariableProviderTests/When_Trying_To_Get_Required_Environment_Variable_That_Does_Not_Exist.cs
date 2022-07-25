using FluentAssertions;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.EnvironmentVariableProviderTests;

public class When_Trying_To_Get_Required_Environment_Variable_That_Does_Not_Exist : SpecificationBase
{
    private IEnvironmentVariableProvider _environmentVariableProvider;

    protected override void Given()
    {
        base.Given();
        _environmentVariableProvider = new EnvironmentVariableProvider();
    }

    [Then]
    public void It_Should_Throw_RequiredVariableNotFoundException()
    {
        _environmentVariableProvider
            .Invoking(provider => provider.GetRequiredEnvironmentVariable("I_DO_NOT_EXIST"))
            .Should().Throw<RequiredEnvironmentVariableNotFoundException>().WithMessage("*I_DO_NOT_EXIST*");
    }
}
