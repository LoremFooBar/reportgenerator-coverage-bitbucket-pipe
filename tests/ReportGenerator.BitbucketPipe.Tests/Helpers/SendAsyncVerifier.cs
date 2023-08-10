using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace ReportGenerator.BitbucketPipe.Tests.Helpers;

public static class SendAsyncVerifier
{
    public static void VerifySendCall(this MockHttpMessageHandler messageHandler, int requiresNumberOfCalls,
        Expression<Predicate<HttpRequestMessage>> requestMatch) => messageHandler.Received(requiresNumberOfCalls)
        .MockSend(Arg.Is(requestMatch), Arg.Any<CancellationToken>());

    public static void VerifySendCall(this MockHttpMessageHandler messageHandler, Quantity requiresQuantityOfCalls,
        Expression<Predicate<HttpRequestMessage>> requestMatch)
    {
        messageHandler.Received(requiresQuantityOfCalls).MockSend(Arg.Is(requestMatch), Arg.Any<CancellationToken>());
    }
}
