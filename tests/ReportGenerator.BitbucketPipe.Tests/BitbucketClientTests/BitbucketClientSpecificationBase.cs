using System.Net.Http;
using Moq;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;

namespace ReportGenerator.BitbucketPipe.Tests.BitbucketClientTests
{
    public class BitbucketClientSpecificationBase : SpecificationBase
    {
        private BitbucketClientMock _bitbucketClientMock;
        protected Mock<HttpMessageHandler> HttpMessageHandlerMock => _bitbucketClientMock.HttpMessageHandlerMock;
        protected BitbucketClient BitbucketClient => _bitbucketClientMock.BitbucketClient;

        protected override void Given()
        {
            base.Given();

            var requirementsOptions = new CoverageRequirementsOptions
                {BranchCoveragePercentageMinimum = 80, LineCoveragePercentageMinimum = 80};
            var bitbucketOptions = new BitbucketOptions
                {ReportTitle = "Code Coverage", BuildStatusName = "Code Coverage"};
            var pipeOptions = new PipeOptions {CreateBuildStatus = true};
            var authOptions = new BitbucketAuthenticationOptions {Username = "user", AppPassword = "pass"};

            _bitbucketClientMock =
                new BitbucketClientMock(requirementsOptions, bitbucketOptions, pipeOptions, authOptions);
        }
    }
}
