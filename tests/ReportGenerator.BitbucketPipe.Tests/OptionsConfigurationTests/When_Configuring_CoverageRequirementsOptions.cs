// using FluentAssertions;
// using Moq;
// using ReportGenerator.BitbucketPipe.Options;
// using ReportGenerator.BitbucketPipe.Tests.BDD;
// using ReportGenerator.BitbucketPipe.Utils;
//
// namespace ReportGenerator.BitbucketPipe.Tests.OptionsConfigurationTests
// {
//     public class When_Configuring_CoverageRequirementsOptions : SpecificationBase
//     {
//         private CoverageRequirementsOptions _options;
//         private IEnvironmentVariableProvider _environmentVariableProvider;
//
//         protected override void Given()
//         {
//             base.Given();
//             _options = new CoverageRequirementsOptions();
//             _environmentVariableProvider = Mock.Of<IEnvironmentVariableProvider>(provider =>
//                 provider.GetEnvironmentVariable(It.Is<string>(s => s == EnvironmentVariable.LineCoverageMinimum)) == "80" &&
//                 provider.GetEnvironmentVariable(It.Is<string>(s => s == EnvironmentVariable.BranchCoverageMinimum)) == "70");
//         }
//
//         protected override void When()
//         {
//             base.When();
//             CoverageRequirementsOptions.Configure(_options, _environmentVariableProvider);
//         }
//
//         [Then]
//         public void It_Should_Configure_Options_Based_On_Provided_Environment_Variables()
//         {
//             _options.BranchCoveragePercentageMinimum.Should().Be(70);
//             _options.LineCoveragePercentageMinimum.Should().Be(80);
//         }
//     }
// }
