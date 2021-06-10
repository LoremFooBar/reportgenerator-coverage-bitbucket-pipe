using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace ReportGenerator.BitbucketPipe.Tests.Helpers
{
    public static class SendAsyncVerifier
    {
        public static void VerifySendAsyncCall(this Mock<HttpMessageHandler> messageHandler, Times times,
            Expression<Func<HttpRequestMessage, bool>> requestMatch) =>
            messageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", times, ItExpr.Is(requestMatch),
                    ItExpr.IsAny<CancellationToken>());
    }
}
