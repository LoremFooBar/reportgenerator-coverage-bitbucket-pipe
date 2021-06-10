using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ReportGenerator.BitbucketPipe.Model.Bitbucket.Report
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Result
    {
        [EnumMember(Value = "PASSED")]
        Passed,

        [EnumMember(Value = "FAILED")]
        Failed,

        [EnumMember(Value = "PENDING")]
        Pending
    }
}
