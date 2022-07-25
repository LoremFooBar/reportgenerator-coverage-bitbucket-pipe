using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace ReportGenerator.BitbucketPipe.Model.Bitbucket.Report;

[Serializable]
[PublicAPI]
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ReportType
{
    [EnumMember(Value = "SECURITY")] Security,

    [EnumMember(Value = "COVERAGE")] Coverage,

    [EnumMember(Value = "TEST")] Test,

    [EnumMember(Value = "BUG")] Bug,
}
