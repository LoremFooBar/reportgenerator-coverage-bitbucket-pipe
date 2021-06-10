// using System.Collections.Generic;
// using System.Linq;
// using JetBrains.Annotations;
// using Moq;
// using ReportGenerator.BitbucketPipe.Utils;
//
// namespace ReportGenerator.BitbucketPipe.Tests.Helpers
// {
//     public class EnvironmentVariableProviderMock
//     {
//         public IEnvironmentVariableProvider Object { get; }
//
//         private Dictionary<string, string> DefaultEnvironment { get; } = new()
//         {
//             ["BITBUCKET_WORKSPACE"] = "workspace",
//             ["BITBUCKET_REPO_SLUG"] = "repo-slug",
//             ["BITBUCKET_COMMIT"] = "f46f058a160a42c68e4b30ee4598cbfc"
//         };
//
//         public EnvironmentVariableProviderMock([CanBeNull] IReadOnlyDictionary<string, string> environment = null)
//         {
//             Dictionary<string, string> unionEnv;
//             if (environment != null) {
//                 var defaultEnv = DefaultEnvironment.Where(kv => !environment.ContainsKey(kv.Key));
//                 unionEnv = environment.Concat(defaultEnv).ToDictionary(kv => kv.Key, kv => kv.Value);
//             }
//             else {
//                 unionEnv = DefaultEnvironment;
//             }
//
//             var envMock = new Mock<IEnvironmentVariableProvider> {CallBase = true};
//             envMock.Setup(_ => _.GetEnvironmentVariable(It.IsAny<string>()))
//                 .Returns((string varName) =>
//                 {
//                     unionEnv.TryGetValue(varName, out string val);
//                     return val;
//                 });
//             Object = envMock.Object;
//         }
//     }
// }
