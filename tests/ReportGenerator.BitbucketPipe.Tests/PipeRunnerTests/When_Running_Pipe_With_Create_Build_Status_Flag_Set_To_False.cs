﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;

namespace ReportGenerator.BitbucketPipe.Tests.PipeRunnerTests;

public class When_Running_Pipe_With_Create_Build_Status_Flag_Set_To_False : SpecificationBase
{
    private MockHttpMessageHandler _messageHandlerMock;
    private TestPipeRunner _pipeRunner;

    protected override void Given()
    {
        base.Given();

        var environment = TestEnvironment.CreateMockEnvironment(new Dictionary<EnvironmentVariable, string>
        {
            [EnvironmentVariable.CreateBuildStatus] = "false",
            [EnvironmentVariable.BitbucketUsername] = "user",
            [EnvironmentVariable.BitbucketAppPassword] = "pass",
            [EnvironmentVariable.Reports] = "**/example.cobertura.xml",
        });

        var bitbucketClientMock = new BitbucketClientMock();
        _messageHandlerMock = bitbucketClientMock.HttpMessageHandlerMock;
        _pipeRunner = new TestPipeRunner(bitbucketClientMock, environment);
    }

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();
        await _pipeRunner.RunPipeAsync();
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
    public void It_Should_Create_Bitbucket_Report()
    {
        _messageHandlerMock.VerifySendCall(1, request =>
            request.RequestUri.PathAndQuery.EndsWith("reports/code-coverage", StringComparison.Ordinal));
    }

    [Then]
    public void It_Should_Not_Create_Build_Status_With_Successful_Status()
    {
        _messageHandlerMock.VerifySendCall(0, request =>
            request.RequestUri.PathAndQuery.EndsWith("statuses/build", StringComparison.Ordinal));
    }
}
