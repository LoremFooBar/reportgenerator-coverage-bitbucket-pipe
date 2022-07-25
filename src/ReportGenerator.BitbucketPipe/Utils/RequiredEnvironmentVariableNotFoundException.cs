using System;

namespace ReportGenerator.BitbucketPipe.Utils;

public class RequiredEnvironmentVariableNotFoundException : Exception
{
    public RequiredEnvironmentVariableNotFoundException(string variableName) :
        base($"Required environment variable {variableName} not found") { }
}
