#nullable disable

using System.Text.Json.Serialization;

namespace Nocturna.Domain.Models.RingCentral;

public class ToDto
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }
}