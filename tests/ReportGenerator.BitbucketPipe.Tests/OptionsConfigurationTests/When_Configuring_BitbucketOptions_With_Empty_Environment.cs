using System.Collections.Generic;
using FluentAssertions;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.OptionsConfigurationTests;

public class When_Configuring_BitbucketOptions_With_Empty_Environment : SpecificationBase
{
    private IEnvironmentVariableProvider _environmentVariableProvider;
    private BitbucketOptions _options;

    protected override void Given()
    {
        base.Given();
        _options = new BitbucketOptions();

        _environmentVariableProvider =
            TestEnvironment.CreateMockEnvironment(new Dictionary<EnvironmentVariable, string>());
    }

    protected override void When()
    {
        base.When();
        BitbucketOptions.Configure(_options, _environmentVariableProvider);
    }

    [Then]
    public void It_Should_Set_Build_Status_Name_And_Report_Title_To_Default_Value()
    {
        _options.ReportTitle.Should().Be("Code Coverage");
        _options.BuildStatusName.Should().Be("Code Coverage");
    }
}
