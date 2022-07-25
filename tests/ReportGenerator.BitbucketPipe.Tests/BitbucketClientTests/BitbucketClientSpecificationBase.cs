using System.Net.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;

namespace ReportGenerator.BitbucketPipe.Tests.BitbucketClientTests;

public class BitbucketClientSpecificationBase : SpecificationBase
{
    private BitbucketClientMock _bitbucketClientMock;
    protected Mock<HttpMessageHandler> HttpMessageHandlerMock => _bitbucketClientMock.HttpMessageHandlerMock;
    protected BitbucketClient BitbucketClient { get; private set; }

    protected override void Given()
    {
        base.Given();

        var requirementsOptions = new CoverageRequirementsOptions
            { BranchCoveragePercentageMinimum = 80, LineCoveragePercentageMinimum = 80 };
        var bitbucketOptions = new BitbucketOptions
            { ReportTitle = "Code Coverage", BuildStatusName = "Code Coverage" };
        var pipeOptions = new PipeOptions { CreateBuildStatus = true };
        var authOptions = new BitbucketAuthenticationOptions { Username = "user", AppPassword = "pass" };

        _bitbucketClientMock =
            new BitbucketClientMock();

        BitbucketClient = new BitbucketClient(
            new HttpClient(_bitbucketClientMock.HttpMessageHandlerMock.Object),
            NullLogger<BitbucketClient>.Instance,
            Mock.Of<IOptions<PublishReportOptions>>(_ => _.Value == new PublishReportOptions()),
            Mock.Of<IOptions<CoverageRequirementsOptions>>(_ => _.Value == requirementsOptions),
            Mock.Of<IOptions<BitbucketOptions>>(_ => _.Value == bitbucketOptions),
            Mock.Of<IOptions<PipeOptions>>(_ => _.Value == pipeOptions),
            Mock.Of<IOptions<BitbucketAuthenticationOptions>>(_ => _.Value == authOptions),
            TestEnvironment.EnvironmentInfo);
    }
}
