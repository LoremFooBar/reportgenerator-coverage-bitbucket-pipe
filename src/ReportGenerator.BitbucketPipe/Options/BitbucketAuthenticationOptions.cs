namespace ReportGenerator.BitbucketPipe.Options;

public class BitbucketAuthenticationOptions
{
    public string? Username { get; set; }
    public string? AppPassword { get; set; }
    public bool UseAuthentication => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(AppPassword);
}
