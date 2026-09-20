using System.Text.Json.Serialization;

namespace SharedKernel.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventCategory
{
    None,
    Academic,
    Social,
    Sports,
    Cultural,
    Career,
    Volunteering,
    Other,
}