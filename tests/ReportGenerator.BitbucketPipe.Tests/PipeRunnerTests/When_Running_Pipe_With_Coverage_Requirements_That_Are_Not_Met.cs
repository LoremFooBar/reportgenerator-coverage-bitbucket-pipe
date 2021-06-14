﻿using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using ReportGenerator.BitbucketPipe.Options;
using ReportGenerator.BitbucketPipe.Tests.BDD;
using ReportGenerator.BitbucketPipe.Tests.Helpers;

namespace ReportGenerator.BitbucketPipe.Tests.PipeRunnerTests
{
    public class When_Running_Pipe_With_Coverage_Requirements_That_Are_Not_Met : SpecificationBase
    {
        private TestPipeRunner _pipeRunner;
        private Mock<HttpMessageHandler> _messageHandlerMock;

        protected override void Given()
        {
            base.Given();

            var environment = TestEnvironment.CreateMockEnvironment(new Dictionary<EnvironmentVariable, string>
            {
                [EnvironmentVariable.BranchCoverageMinimum] = "95",
                [EnvironmentVariable.LineCoverageMinimum] = "95",

                [EnvironmentVariable.BitbucketUsername] = "user",
                [EnvironmentVariable.BitbucketAppPassword] = "pass"
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
        public void It_Should_Create_Bitbucket_Report_With_Failed_Status()
        {
            _messageHandlerMock.VerifySendAsyncCall(Times.Once(), request =>
                request.RequestUri.PathAndQuery.EndsWith("reports/code-coverage") &&
                request.Content.ReadAsStringAsync().Result.Contains("\"result\":\"FAILED\""));
        }

        [Then]
        public void It_Should_Create_Build_Status_With_Failed_Status()
        {
            _messageHandlerMock.VerifySendAsyncCall(Times.Once(), request =>
                request.RequestUri.PathAndQuery.EndsWith("statuses/build") &&
                request.Content.ReadAsStringAsync().Result.Contains("\"state\":\"FAILED\""));
        }
    }
}