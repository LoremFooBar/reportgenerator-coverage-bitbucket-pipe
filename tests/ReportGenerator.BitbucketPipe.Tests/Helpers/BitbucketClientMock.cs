using System.Net;
using System.Net.Http;
using System.Threading;
using NSubstitute;

namespace ReportGenerator.BitbucketPipe.Tests.Helpers;

public class BitbucketClientMock
{
    public BitbucketClientMock()
    {
        HttpMessageHandlerMock = Substitute.ForPartsOf<MockHttpMessageHandler>();
        HttpMessageHandlerMock.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.OK));
    }

    public MockHttpMessageHandler HttpMessageHandlerMock { get; }
}
