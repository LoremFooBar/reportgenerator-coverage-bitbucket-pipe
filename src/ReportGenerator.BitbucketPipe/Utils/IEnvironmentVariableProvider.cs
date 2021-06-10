namespace ReportGenerator.BitbucketPipe.Utils
{
    public interface IEnvironmentVariableProvider
    {
        string? GetEnvironmentVariable(string variableName);

        string GetRequiredEnvironmentVariable(string variableName) =>
            GetEnvironmentVariable(variableName) ??
            throw new RequiredEnvironmentVariableNotFoundException(variableName);

        string GetEnvironmentVariableOrDefault(string variableName, string defaultValue) =>
            GetEnvironmentVariable(variableName) ?? defaultValue;
    }
}
