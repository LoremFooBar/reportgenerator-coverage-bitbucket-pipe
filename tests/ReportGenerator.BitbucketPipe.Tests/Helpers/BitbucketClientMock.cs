using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace ReportGenerator.BitbucketPipe.Tests.Helpers
{
    public class BitbucketClientMock
    {
        //public BitbucketClient BitbucketClient { get; }
        public Mock<HttpMessageHandler> HttpMessageHandlerMock { get; }

        public BitbucketClientMock()
        {
            HttpMessageHandlerMock = new Mock<HttpMessageHandler>();
            HttpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            // var httpClient = new HttpClient(HttpMessageHandlerMock.Object);
            //
            // var publishReportOptions =
            //     Mock.Of<IOptions<PublishReportOptions>>(options => options.Value == new PublishReportOptions());
            //
            // var requirementsOptionsMock =
            //     Mock.Of<IOptions<CoverageRequirementsOptions>>(options => options.Value == requirementsOptions);
            //
            // var bitbucketOptionsMock =
            //     Mock.Of<IOptions<BitbucketOptions>>(options => options.Value == bitbucketOptions);
            //
            // var authOptionsMock = Mock.Of<IOptions<BitbucketAuthenticationOptions>>(options =>
            //     options.Value == (authOptions ?? new BitbucketAuthenticationOptions()));
            //
            // var pipeOptionsMock = Mock.Of<IOptions<PipeOptions>>(options => options.Value == pipeOptions);

            // BitbucketClient = new BitbucketClient(httpClient, NullLogger<BitbucketClient>.Instance,
            //     publishReportOptions, requirementsOptionsMock, bitbucketOptionsMock, pipeOptionsMock, authOptionsMock,
            //     environmentInfo ?? TestEnvironment.EnvironmentInfo);
        }
    }
}
