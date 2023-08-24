using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;

namespace ReportGenerator.BitbucketPipe.Tests.PipeRunnerTests;

public class
    When_Running_Pipe_With_Coverage_Requirements_That_Are_Met_And_FailWhenMinimumNotMet_Is_True : SpecificationBase
{
    private ExitCode _exitCode;
    private MockHttpMessageHandler _messageHandlerMock;
    private TestPipeRunner _pipeRunner;

    protected override void Given()
    {
        base.Given();

        var environment = TestEnvironment.CreateMockEnvironment(new Dictionary<EnvironmentVariable, string>
        {
            [EnvironmentVariable.BitbucketUsername] = "user",
            [EnvironmentVariable.BitbucketAppPassword] = "pass",

            [EnvironmentVariable.LineCoverageMinimum] = "50",
            [EnvironmentVariable.BranchCoverageMinimum] = "50",

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
    public void It_Should_Create_Report_In_Destination_Directory()
    {
        var directory = new DirectoryInfo(ReportGeneratorOptions.DefaultDestinationPath);
        directory.Exists.Should().BeTrue();
        directory.GetFiles("*.htm*").Should().NotBeEmpty();
        directory.GetFiles("Summary*.json").Should().NotBeEmpty();
    }

    [Then]
    public void It_Should_Create_Bitbucket_Report_With_Passed_Status()
    {
        _messageHandlerMock.Received(1).MockSend(Arg.Is<HttpRequestMessage>(request =>
                request.RequestUri.PathAndQuery.EndsWith("reports/code-coverage", StringComparison.Ordinal) &&
                request.Content.ReadAsStringAsync().Result.Contains("\"result\":\"PASSED\"")),
            Arg.Any<CancellationToken>());
    }

    [Then]
    public void It_Should_Create_Build_Status_With_Successful_Status()
    {
        _messageHandlerMock.VerifySendCall(1, request =>
            request.RequestUri.PathAndQuery.EndsWith("statuses/build", StringComparison.Ordinal) &&
            request.Content.ReadAsStringAsync().Result.Contains("\"state\":\"SUCCESSFUL\""));
    }

    [Then]
    public void It_Should_Return_Exit_Code_0()
    {
        _exitCode.Code.Should().Be(0);
    }
}
