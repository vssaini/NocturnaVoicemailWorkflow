#nullable disable

using System.Text.Json.Serialization;

namespace Nocturna.Domain.Models.RingCentral;

public class PhoneNumberInfoDto
{
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; }

    [JsonPropertyName("nationalDestinationCode")]
    public string NationalDestinationCode { get; set; }

    [JsonPropertyName("subscriberNumber")]
    public string SubscriberNumber { get; set; }
}