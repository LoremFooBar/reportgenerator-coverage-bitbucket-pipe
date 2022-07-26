﻿using System;
using ReportGenerator.BitbucketPipe.Utils;

namespace ReportGenerator.BitbucketPipe.Options;

public class PublishReportOptions
{
    public Uri? ReportUrl { get; private set; }

    public static void Configure(PublishReportOptions options, IEnvironmentVariableProvider envVariableProvider)
    {
        string? reportUrlStr = envVariableProvider.GetEnvironmentVariable(EnvironmentVariable.PublishedReportUrl);
        Uri.TryCreate(reportUrlStr, UriKind.Absolute, out var reportUrl);
        options.ReportUrl = reportUrl;
    }
}
