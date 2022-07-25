using System.Collections.Generic;
using FluentAssertions;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.OptionsConfigurationTests;

public class When_Configuring_BitbucketOptions_Without_Report_Title : SpecificationBase
{
    private IEnvironmentVariableProvider _environmentVariableProvider;
    private BitbucketOptions _options;

    protected override void Given()
    {
        base.Given();
        _options = new BitbucketOptions();

        _environmentVariableProvider = TestEnvironment.CreateMockEnvironment(new Dictionary<EnvironmentVariable, string>
        {
            [EnvironmentVariable.BuildStatusName] = "My Code Coverage",
        });
    }

    protected override void When()
    {
        base.When();
        BitbucketOptions.Configure(_options, _environmentVariableProvider);
    }

    [Then]
    public void It_Should_Set_Report_Title_To_Same_Value_As_Build_Status_Name()
    {
        _options.ReportTitle.Should().Be("My Code Coverage");
        _options.BuildStatusName.Should().Be("My Code Coverage");
    }
}
