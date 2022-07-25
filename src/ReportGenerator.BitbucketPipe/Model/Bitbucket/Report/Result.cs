using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace ReportGenerator.BitbucketPipe.Model.Bitbucket.Report;

[Serializable]
[PublicAPI]
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum Result
{
    [EnumMember(Value = "PASSED")] Passed,

    [EnumMember(Value = "FAILED")] Failed,

    [EnumMember(Value = "PENDING")] Pending,
}
