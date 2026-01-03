using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReportGenerator.BitbucketPipe.Model;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Utils;
using Serilog;

namespace ReportGenerator.BitbucketPipe;

public class PipeRunner
{
    private readonly IEnvironmentVariableProvider _environmentVariableProvider;
    private readonly PipeEnvironment _pipeEnvironment;

    public PipeRunner(IEnvironmentVariableProvider environmentVariableProvider)
    {
        _environmentVariableProvider = environmentVariableProvider;
        _pipeEnvironment = new PipeEnvironment(_environmentVariableProvider);
    }

    public async Task<ExitCode> RunPipeAsync()
    {
        var services = ConfigureServices();
        var serviceProvider = services.BuildServiceProvider();

        var coverageReportGenerator = serviceProvider.GetRequiredService<CoverageReportGenerator>();
        var coverageSummary = await coverageReportGenerator.GenerateCoverageReportAsync();

        var bitbucketClient = serviceProvider.GetRequiredService<BitbucketClient>();
        await bitbucketClient.CreateCommitBuildStatusAsync(coverageSummary);
        await bitbucketClient.CreateReportAsync(coverageSummary);

        return GetExitCode(serviceProvider, coverageSummary);
    }

    private static ExitCode GetExitCode(IServiceProvider serviceProvider, CoverageSummary coverageSummary)
    {
        if (!serviceProvider.GetRequiredService<IOptions<PipeOptions>>().Value.FailWhenMinimumNotMet)
            return ExitCode.Success;

        var coverageRequirementsOptions =
            serviceProvider.GetRequiredService<IOptions<CoverageRequirementsOptions>>().Value;

        return RequirementsChecker.CoverageMeetsRequirements(coverageRequirementsOptions, coverageSummary)
            ? ExitCode.Success
            : ExitCode.MinimumNotMet;
    }

    protected virtual IServiceCollection ConfigureServices()
    {
        IServiceCollection services = new ServiceCollection();

        var authOptions = new BitbucketAuthenticationOptions
        {
            AccountEmail =
                _environmentVariableProvider.GetRequiredEnvironmentVariable(EnvironmentVariable.AccountEmail),
            ApiToken = _environmentVariableProvider.GetRequiredEnvironmentVariable(EnvironmentVariable.ApiToken),
        };

        SetupBitbucketClient(services, authOptions);

        services
            .AddSingleton<CoverageReportGenerator>()
            .AddLogging(builder => builder.AddSerilog())
            .AddSingleton<IEnvironmentVariableProvider, EnvironmentVariableProvider>()
            .Configure<CoverageRequirementsOptions>(options =>
                CoverageRequirementsOptions.Configure(options, _environmentVariableProvider))
            .Configure<PublishReportOptions>(options =>
                PublishReportOptions.Configure(options, _environmentVariableProvider))
            .Configure<ReportGeneratorOptions>(options =>
                ReportGeneratorOptions.Configure(options, _environmentVariableProvider))
            .Configure<BitbucketOptions>(options =>
                BitbucketOptions.Configure(options, _environmentVariableProvider))
            .Configure<PipeOptions>(options => PipeOptions.Configure(options, _environmentVariableProvider))
            .Configure<BitbucketAuthenticationOptions>(options =>
            {
                options.AccountEmail = authOptions.AccountEmail;
                options.ApiToken = authOptions.ApiToken;
            })
            .AddSingleton(_pipeEnvironment)
            .AddSingleton<BitbucketEnvironmentInfo>();

        return services;
    }

    private static void SetupBitbucketClient(IServiceCollection services, BitbucketAuthenticationOptions authOptions)
    {
        var httpClientBuilder = services.AddHttpClient<BitbucketClient>();

        Log.Debug("Authenticating using API token and email");
        httpClientBuilder.ConfigureHttpClient(client =>
        {
            string authenticationString = $"{authOptions.AccountEmail}:{authOptions.ApiToken}";
            string base64EncodedAuthenticationString =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        });
    }
}
