using FluentAssertions;
using Moq;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.OptionsConfigurationTests
{
    public class When_Configuring_BitbucketOptions : SpecificationBase
    {
        private BitbucketOptions _options;
        private IEnvironmentVariableProvider _environmentVariableProvider;

        protected override void Given()
        {
            base.Given();
            _options = new BitbucketOptions();
            _environmentVariableProvider = Mock.Of<IEnvironmentVariableProvider>(provider =>
                provider.GetEnvironmentVariable(It.Is<string>(s => s == EnvironmentVariable.BuildStatusName)) ==
                "My Coverage Status" &&
                provider.GetEnvironmentVariable(It.Is<string>(s => s == EnvironmentVariable.PipelineReportTitle)) ==
                "My Coverage Report");
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
}
