using System.Collections.Generic;
using FluentAssertions;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.OptionsConfigurationTests;

public class When_Configuring_ReportGeneratorOptions_With_Extra_Arguments : SpecificationBase
{
    private IEnvironmentVariableProvider _environment;
    private ReportGeneratorOptions _options;

    protected override void Given()
    {
        base.Given();
        _options = new ReportGeneratorOptions();
        _environment = TestEnvironment.CreateMockEnvironment(new Dictionary<EnvironmentVariable, string>
        {
            [EnvironmentVariable.ExtraArgs] = "not-empty",
            [EnvironmentVariable.ExtraArgsCount] = "4",
            [EnvironmentVariable.ExtraArgs + "_0"] = "arg1",
            [EnvironmentVariable.ExtraArgs + "_1"] = "arg2",
            [EnvironmentVariable.ExtraArgs + "_2"] = "arg3",
            [EnvironmentVariable.ExtraArgs + "_3"] = "arg4",
        });
    }

    protected override void When()
    {
        base.When();
        ReportGeneratorOptions.Configure(_options, _environment);
    }

    [Then]
    public void It_Should_Populate_Extra_Args()
    {
        _options.ExtraArguments.Should()
            .HaveCount(4).And
            .ContainInOrder("arg1", "arg2", "arg3", "arg4");
    }
}
