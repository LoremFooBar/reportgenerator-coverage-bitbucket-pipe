using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReportGenerator.BitbucketPipe.Tests.Helpers;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Tests.PipeRunnerTests;

public class TestPipeRunner : PipeRunner
{
    private readonly BitbucketClientMock _bitbucketClientMock;
    private readonly IEnvironmentVariableProvider _environmentVariableProvider;

    public TestPipeRunner(BitbucketClientMock bitbucketClientMock,
        IEnvironmentVariableProvider environmentVariableProvider) : base(environmentVariableProvider)
    {
        _bitbucketClientMock = bitbucketClientMock;
        _environmentVariableProvider = environmentVariableProvider;
    }

    protected override IServiceCollection ConfigureServices()
    {
        var services = base.ConfigureServices();

        // add mock BitbucketClient
        var bitbucketClientService =
            services.FirstOrDefault(service => service.ServiceType == typeof(BitbucketClient));
        services.Remove(bitbucketClientService);
        services.AddHttpClient<BitbucketClient>()
            .ConfigurePrimaryHttpMessageHandler(() => _bitbucketClientMock.HttpMessageHandlerMock.Object);

        // add mock IEnvironmentVariablesProvider
        var environmentVariableProviderService =
            services.FirstOrDefault(service => service.ServiceType == typeof(IEnvironmentVariableProvider));
        services.Remove(environmentVariableProviderService);
        services.AddSingleton(_environmentVariableProvider);

        return services;
    }
}
