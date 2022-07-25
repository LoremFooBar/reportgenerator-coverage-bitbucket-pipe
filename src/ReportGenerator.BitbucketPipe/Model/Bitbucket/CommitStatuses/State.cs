using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace ReportGenerator.BitbucketPipe.Model.Bitbucket.CommitStatuses;

[PublicAPI]
[Serializable]
[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum State
{
    [EnumMember(Value = "SUCCESSFUL")] Successful,

    [EnumMember(Value = "FAILED")] Failed,

    [EnumMember(Value = "INPROGRESS")] Inprogress,

    [EnumMember(Value = "STOPPED")] Stopped,
}
