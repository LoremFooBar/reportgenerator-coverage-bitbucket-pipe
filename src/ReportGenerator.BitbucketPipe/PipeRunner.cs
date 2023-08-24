using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
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
            Username = _environmentVariableProvider.GetEnvironmentVariable(EnvironmentVariable.BitbucketUsername),
            AppPassword = _environmentVariableProvider.GetEnvironmentVariable(EnvironmentVariable.BitbucketAppPassword),
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
                options.Username = authOptions.Username;
                options.AppPassword = authOptions.AppPassword;
            })
            .AddSingleton(_pipeEnvironment)
            .AddSingleton<BitbucketEnvironmentInfo>();

        return services;
    }

    private void SetupBitbucketClient(IServiceCollection services, BitbucketAuthenticationOptions authOptions)
    {
        var httpClientBuilder = services.AddHttpClient<BitbucketClient>();

        if (authOptions.UseAuthentication) {
            Log.Debug("Authenticating using app password");
            httpClientBuilder.ConfigureHttpClient(client =>
                client.SetBasicAuthentication(authOptions.Username, authOptions.AppPassword));
        }
        else if (!_pipeEnvironment.IsDevelopment) {
            // set proxy for pipe when running in pipelines
            const string proxyUrl = "http://host.docker.internal:29418";

            Log.Debug("Using proxy {Proxy}", proxyUrl);
            Log.Information("Not using authentication - can't create build status");
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                { Proxy = new WebProxy(proxyUrl) });
        }
        else
            Log.Error("Could not authenticate to Bitbucket!");
    }
}
