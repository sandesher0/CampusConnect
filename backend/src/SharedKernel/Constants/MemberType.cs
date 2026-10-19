using System.Text.Json.Serialization;

namespace SharedKernel.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MemberType
{
    None,
    ClubOfficer,
    Student,
    ClubMember,
    StudentAffairsStaff
}