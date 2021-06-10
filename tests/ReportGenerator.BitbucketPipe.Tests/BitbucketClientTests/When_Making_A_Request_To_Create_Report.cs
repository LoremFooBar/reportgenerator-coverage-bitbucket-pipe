using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using ReportGenerator.BitbucketPipe.Model;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;

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
            HttpMessageHandlerMock.VerifySendAsyncCall(Times.Once(), request =>
                request.Method == HttpMethod.Put &&
                request.RequestUri.PathAndQuery.EndsWith(
                    "workspace/repo-slug/commit/222be690/reports/code-coverage"));
        }

        [Then]
        public void It_Should_Serialize_Report_Using_Snake_Case()
        {
            HttpMessageHandlerMock.VerifySendAsyncCall(Times.AtLeastOnce(),
                request => request.Content.ReadAsStringAsync().Result.Contains("\"report_type\":"));
        }
    }
}
