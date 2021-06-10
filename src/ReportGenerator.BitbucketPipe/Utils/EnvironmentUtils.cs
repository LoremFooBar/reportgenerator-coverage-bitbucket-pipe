using System;
using static System.Environment;

namespace ReportGenerator.BitbucketPipe.Utils
{
    internal static class EnvironmentUtils
    {
        public static bool IsDebugMode { get; } =
            GetEnvironmentVariable("DEBUG")?.Equals("true", StringComparison.OrdinalIgnoreCase)
            ?? false;

        public static string EnvironmentName { get; } =
            GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "Production";

        public static bool IsDevelopment { get; } =
            EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase);
    }
}
