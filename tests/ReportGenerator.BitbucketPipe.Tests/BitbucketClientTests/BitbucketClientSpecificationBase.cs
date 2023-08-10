using System.Net.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;

namespace ReportGenerator.BitbucketPipe.Tests.BitbucketClientTests;

public class BitbucketClientSpecificationBase : SpecificationBase
{
    private BitbucketClientMock _bitbucketClientMock;
    protected MockHttpMessageHandler HttpMessageHandlerMock => _bitbucketClientMock.HttpMessageHandlerMock;
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

        _bitbucketClientMock = new BitbucketClientMock();

        BitbucketClient = new BitbucketClient(
            new HttpClient(_bitbucketClientMock.HttpMessageHandlerMock),
            NullLogger<BitbucketClient>.Instance,
            new OptionsWrapper<PublishReportOptions>(new PublishReportOptions()),
            new OptionsWrapper<CoverageRequirementsOptions>(requirementsOptions),
            new OptionsWrapper<BitbucketOptions>(bitbucketOptions),
            new OptionsWrapper<PipeOptions>(pipeOptions),
            new OptionsWrapper<BitbucketAuthenticationOptions>(authOptions),
            TestEnvironment.EnvironmentInfo);
    }
}
