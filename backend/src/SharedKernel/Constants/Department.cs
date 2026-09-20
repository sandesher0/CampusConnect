
using System.Text.Json.Serialization;

namespace SharedKernel.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Department
{
    None,
    ComputerScience,
    Management,
    Economics,
    Maths,

}