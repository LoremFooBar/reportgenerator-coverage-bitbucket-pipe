using System;

namespace ReportGenerator.BitbucketPipe.Utils
{
    public class EnvironmentVariableProvider : IEnvironmentVariableProvider
    {
        public string? GetEnvironmentVariable(string variableName) => Environment.GetEnvironmentVariable(variableName);
    }
}
