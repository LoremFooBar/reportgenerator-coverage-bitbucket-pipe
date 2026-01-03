using System;

namespace ReportGenerator.BitbucketPipe.Options;

[Serializable]
public class BitbucketAuthenticationOptions
{
    public required string AccountEmail { get; set; }
    public required string ApiToken { get; set; }
}
