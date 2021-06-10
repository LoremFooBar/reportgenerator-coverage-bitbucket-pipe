using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using ReportGenerator.BitbucketPipe.Model;
using ReportGenerator.BitbucketPipe.Tests.BDD;

namespace ReportGenerator.BitbucketPipe.Tests.BitbucketClientTests
{
    public class When_Making_A_Request_To_Create_Build_Status : BitbucketClientSpecificationBase
    {
        protected override async Task WhenAsync()
        {
            await base.WhenAsync();

            await BitbucketClient.CreateCommitBuildStatusAsync(new CoverageSummary
                {BranchCoveragePercentage = 85, LineCoveragePercentage = 85});
        }

        [Then]
        public void It_Should_Make_One_Post_Call_To_Create_Build_Status()
        {
            HttpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(message =>
                    message.Method == HttpMethod.Post &&
                    message.RequestUri.PathAndQuery.EndsWith("workspace/repo-slug/commit/222be690/statuses/build")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
