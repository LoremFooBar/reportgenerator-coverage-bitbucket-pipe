using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using ReportGenerator.BitbucketPipe.Model;
using ReportGenerator.BitbucketPipe.Tests.BDD;

namespace ReportGenerator.BitbucketPipe.Tests.BitbucketClientTests
{
    public class When_Making_A_Request_To_Create_Report : BitbucketClientSpecificationBase
    {
        protected override async Task WhenAsync()
        {
            await base.WhenAsync();

            var coverageSummary = new CoverageSummary {BranchCoveragePercentage = 85, LineCoveragePercentage = 85};
            await BitbucketClient.CreateReportAsync(coverageSummary);
        }

        [Then]
        public void It_Should_Make_One_Put_Call_To_Create_Report()
        {
            HttpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(message =>
                    message.Method == HttpMethod.Put &&
                    message.RequestUri.PathAndQuery.EndsWith(
                        $"workspace/repo-slug/commit/222be690/reports/code-coverage")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
