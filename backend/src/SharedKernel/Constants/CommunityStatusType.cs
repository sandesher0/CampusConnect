using System.Text.Json.Serialization;

namespace SharedKernel.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommunityStatusType
{
    None,
    Active,
    Suspended,
    Inactive,
    Closed

}