namespace ReportGenerator.BitbucketPipe;

public class ExitCode
{
    private ExitCode(int code) => Code = code;

    public static ExitCode Success { get; } = new(0);
    public static ExitCode MinimumNotMet { get; } = new(12);

    public int Code { get; }

    public static implicit operator int(ExitCode exitCode) => exitCode.Code;
}
