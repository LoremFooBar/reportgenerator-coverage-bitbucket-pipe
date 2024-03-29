﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ReportGenerator.BitbucketPipe.Tests.Helpers;

public abstract class MockHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken) => Task.FromResult(MockSend(request, cancellationToken));

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) =>
        MockSend(request, cancellationToken);

    public abstract HttpResponseMessage MockSend(HttpRequestMessage request, CancellationToken cancellationToken);
}
