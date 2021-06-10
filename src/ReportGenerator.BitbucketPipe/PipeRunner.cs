using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Utils;
using Serilog;

namespace ReportGenerator.BitbucketPipe
{
    public class PipeRunner
    {
        private readonly IEnvironmentVariableProvider _environmentVariableProvider;

        public PipeRunner(IEnvironmentVariableProvider environmentVariableProvider) =>
            _environmentVariableProvider = environmentVariableProvider;

        public async Task RunPipeAsync()
        {
            var serviceProvider = await ConfigureServicesAsync();

            var coverageReportGenerator = serviceProvider.GetRequiredService<CoverageReportGenerator>();
            var coverageSummary = await coverageReportGenerator.GenerateCoverageReportAsync();

            var bitbucketClient = serviceProvider.GetRequiredService<BitbucketClient>();
            await bitbucketClient.CreateCommitBuildStatusAsync(coverageSummary);
            await bitbucketClient.CreateReportAsync(coverageSummary);
        }

        protected virtual async Task<ServiceProvider> ConfigureServicesAsync()
        {
            string accessToken = await GetAccessTokenAsync();

            var serviceCollection =
                new ServiceCollection()
                    .AddSingleton<CoverageReportGenerator>()
                    .AddHttpClient<BitbucketClient>(client =>
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue(
                                OidcConstants.AuthenticationSchemes.AuthorizationHeaderBearer,
                                accessToken)).Services
                    .AddLogging(builder => builder.AddSerilog())
                    .Configure<CoverageRequirementsOptions>(CoverageRequirementsOptions.Configure)
                    .Configure<PublishReportOptions>(PublishReportOptions.Configure)
                    .Configure<ReportGeneratorOptions>(options =>
                        ReportGeneratorOptions.Configure(options, _environmentVariableProvider))
                    .Configure<BitbucketOptions>(BitbucketOptions.Configure);

            return serviceCollection.BuildServiceProvider();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var authenticationOptions = new BitbucketAuthenticationOptions
            {
                Key = _environmentVariableProvider.GetRequiredEnvironmentVariable("BITBUCKET_OAUTH_KEY"),
                Secret = _environmentVariableProvider.GetRequiredEnvironmentVariable("BITBUCKET_OAUTH_SECRET")
            };

            Log.Debug("Getting access token...");

            using var httpClient = new HttpClient();
            var tokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = authenticationOptions.Key,
                ClientSecret = authenticationOptions.Secret,
                Scope = "repository:write",
                Address = "https://bitbucket.org/site/oauth2/access_token"
            };

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);

            if (!tokenResponse.IsError) {
                Log.Debug("Got access token");
                return tokenResponse.AccessToken;
            }

            Log.Error("Error getting access token: {@Error}",
                new
                {
                    tokenResponse.Error, tokenResponse.ErrorDescription, tokenResponse.ErrorType,
                    tokenResponse.HttpStatusCode
                });

            throw new OAuthException(tokenResponse);
        }
    }
}
