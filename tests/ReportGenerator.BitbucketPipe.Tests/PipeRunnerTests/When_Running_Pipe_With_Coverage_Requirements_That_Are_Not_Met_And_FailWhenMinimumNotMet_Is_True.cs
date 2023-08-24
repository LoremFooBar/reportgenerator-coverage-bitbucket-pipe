using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;

namespace ReportGenerator.BitbucketPipe.Tests.PipeRunnerTests;

public class
    When_Running_Pipe_With_Coverage_Requirements_That_Are_Not_Met_And_FailWhenMinimumNotMet_Is_True : SpecificationBase
{
    private ExitCode _exitCode;
    private MockHttpMessageHandler _messageHandlerMock;
    private TestPipeRunner _pipeRunner;

    protected override void Given()
    {
        base.Given();

        var environment = TestEnvironment.CreateMockEnvironment(new Dictionary<EnvironmentVariable, string>
        {
            [EnvironmentVariable.BranchCoverageMinimum] = "95",
            [EnvironmentVariable.LineCoverageMinimum] = "95",

            [EnvironmentVariable.BitbucketUsername] = "user",
            [EnvironmentVariable.BitbucketAppPassword] = "pass",

            [EnvironmentVariable.FailWhenMinimumNotMet] = "true",

            [EnvironmentVariable.Reports] = "**/example.cobertura.xml",
        });

        var bitbucketClientMock = new BitbucketClientMock();
        _messageHandlerMock = bitbucketClientMock.HttpMessageHandlerMock;
        _pipeRunner = new TestPipeRunner(bitbucketClientMock, environment);
    }

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();
        _exitCode = await _pipeRunner.RunPipeAsync();
    }

    [Then]
    public void It_Should_Create_Bitbucket_Report_With_Failed_Status()
    {
        _messageHandlerMock.VerifySendCall(1, request =>
            request.RequestUri.PathAndQuery.EndsWith("reports/code-coverage", StringComparison.Ordinal) &&
            request.Content.ReadAsStringAsync().Result.Contains("\"result\":\"FAILED\""));
    }

    [Then]
    public void It_Should_Create_Build_Status_With_Failed_Status()
    {
        _messageHandlerMock.VerifySendCall(1, request =>
            request.RequestUri.PathAndQuery.EndsWith("statuses/build", StringComparison.Ordinal) &&
            request.Content.ReadAsStringAsync().Result.Contains("\"state\":\"FAILED\""));
    }

    [Then]
    public void It_Should_Return_Exit_Code_12()
    {
        _exitCode.Code.Should().Be(12);
    }
}
