using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using ReportGenerator.BitbucketPipe.Model;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.ReportGeneratorTests
{
    public class When_Generating_Report : SpecificationBase
    {
        private CoverageReportGenerator _coverageReportGenerator;
        private CoverageSummary _coverageSummary;
        private ReportGeneratorOptions _reportGeneratorOptions;

        protected override void Given()
        {
            base.Given();
            _reportGeneratorOptions = new ReportGeneratorOptions
            {
                Reports =
                    $"ExampleCoverageTestResults{Path.DirectorySeparatorChar}*{Path.DirectorySeparatorChar}example.cobertura.xml",
                ReportTypes = "JsonSummary;Html"
            };
            var reportGeneratorOptions =
                Mock.Of<IOptions<ReportGeneratorOptions>>(options => options.Value == _reportGeneratorOptions);

            var environmentVariableProviderMock = new Mock<IEnvironmentVariableProvider> {CallBase = true};
            environmentVariableProviderMock
                .Setup(provider => provider.GetEnvironmentVariable(It.IsAny<string>()))
                .Returns((string) null);

            _coverageReportGenerator = new CoverageReportGenerator(NullLogger<CoverageReportGenerator>.Instance,
                reportGeneratorOptions, new PipeEnvironment(environmentVariableProviderMock.Object));
        }

        protected override async Task WhenAsync()
        {
            await base.WhenAsync();
            _coverageSummary = await _coverageReportGenerator.GenerateCoverageReportAsync();
        }

        [Then]
        public void It_Should_Correctly_Parse_Coverage_Summary()
        {
            _coverageSummary.Should().NotBeNull();
            _coverageSummary.BranchCoveragePercentage.Should().BeGreaterOrEqualTo(75);
            _coverageSummary.LineCoveragePercentage.Should().BeGreaterOrEqualTo(92.7);
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            Directory.Delete(_reportGeneratorOptions.DestinationPath, true);
        }
    }
}
