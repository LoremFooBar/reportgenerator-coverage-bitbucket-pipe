using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace ReportGenerator.BitbucketPipe.Tests.Helpers;

public class BitbucketClientMock
{
    public BitbucketClientMock()
    {
        HttpMessageHandlerMock = new Mock<HttpMessageHandler>();
        HttpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
    }

    public Mock<HttpMessageHandler> HttpMessageHandlerMock { get; }
}
