using FluentAssertions;
using NSubstitute;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.OptionsConfigurationTests;

public class When_Configuring_BitbucketOptions : SpecificationBase
{
    private IEnvironmentVariableProvider _environmentVariableProvider;
    private BitbucketOptions _options;

    protected override void Given()
    {
        base.Given();
        _options = new BitbucketOptions();
        _environmentVariableProvider = Substitute.ForPartsOf<DefaultEnvironmentVariableProvider>();

        _environmentVariableProvider.GetEnvironmentVariable(Arg.Any<string>()).Returns(
            x =>
            {
                string varName = (string)x[0];

                return varName == EnvironmentVariable.BuildStatusName ? "My Coverage Status" :
                    varName == EnvironmentVariable.PipelineReportTitle ? "My Coverage Report" :
                    "";
            });
    }

    protected override void When()
    {
        base.When();
        BitbucketOptions.Configure(_options, _environmentVariableProvider);
    }

    [Then]
    public void It_Should_Configure_BitbucketOptions_Based_On_Provided_Environment_Variables()
    {
        _options.ReportTitle.Should().Be("My Coverage Report");
        _options.BuildStatusName.Should().Be("My Coverage Status");
    }
}
