#nullable disable

using System.Text.Json.Serialization;

namespace Nocturna.Domain.Models.RingCentral;

public class FromDto
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("phoneNumberInfo")]
    public PhoneNumberInfoDto PhoneNumberInfo { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}