// using FluentAssertions;
// using Moq;
// using ReportGenerator.BitbucketPipe.Options;
// using ReportGenerator.BitbucketPipe.Tests.BDD;
// using ReportGenerator.BitbucketPipe.Utils;
//
// namespace ReportGenerator.BitbucketPipe.Tests.OptionsConfigurationTests
// {
//     public class When_Configuring_Pipe_Options : SpecificationBase
//     {
//         private PipeOptions _options;
//         private Mock<IEnvironmentVariableProvider> _envVarProviderMock;
//
//         protected override void Given()
//         {
//             base.Given();
//             _options = new PipeOptions();
//             _envVarProviderMock = new Mock<IEnvironmentVariableProvider> {CallBase = true};
//             _envVarProviderMock
//                 .Setup(provider => provider.GetEnvironmentVariable(It.Is<string>(s => s == "CREATE_BUILD_STATUS")))
//                 .Returns("false");
//         }
//
//         protected override void When()
//         {
//             base.When();
//             PipeOptions.Configure(_options, _envVarProviderMock.Object);
//         }
//
//         [Then]
//         public void It_Should_Configure_PipeOptions_Based_On_Provided_Environment_Variables()
//         {
//             _options.CreateBuildStatus.Should().BeFalse();
//         }
//     }
// }
