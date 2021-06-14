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
    public class When_Generating_Report_Filtering_Out_All_Files : SpecificationBase
    {
        private CoverageReportGenerator _coverageReportGenerator;
        private CoverageSummary _coverageSummary;
        private ReportGeneratorOptions _reportGeneratorOptions;

        protected override void Given()
        {
            base.Given();
            _reportGeneratorOptions = new ReportGeneratorOptions
            {
                Reports = $"**{Path.DirectorySeparatorChar}coverage*.xml",
                ReportTypes = "JsonSummary;Html",
                ExtraArguments = new []{$"-fileFilters:+IDoNotExist.cs"}
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
        public void It_Should_Correctly_Parse_Coverage_Summary_Taking_In_Account_Extra_Arguments()
        {
            _coverageSummary.Should().NotBeNull();
            _coverageSummary.BranchCoveragePercentage.Should().Be(0);
            _coverageSummary.LineCoveragePercentage.Should().Be(0);
        }

    }
}
