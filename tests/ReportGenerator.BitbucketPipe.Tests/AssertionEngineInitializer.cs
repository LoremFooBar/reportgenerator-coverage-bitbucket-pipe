using FluentAssertions;
using ReportGenerator.BitbucketPipe.Tests;

[assembly: FluentAssertions.Extensibility.AssertionEngineInitializer(
    typeof(AssertionEngineInitializer),
    nameof(AssertionEngineInitializer.AcknowledgeSoftWarning))]

namespace ReportGenerator.BitbucketPipe.Tests;

public static class AssertionEngineInitializer
{
    public static void AcknowledgeSoftWarning()
    {
        License.Accepted = true;
    }
}
