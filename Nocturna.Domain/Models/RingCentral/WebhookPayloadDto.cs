#nullable disable

using System.Text.Json.Serialization;

namespace Nocturna.Domain.Models.RingCentral;

public class WebhookPayloadDto
{
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }

    [JsonPropertyName("event")]
    public string Event { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("subscriptionId")]
    public string SubscriptionId { get; set; }

    [JsonPropertyName("ownerId")]
    public string OwnerId { get; set; }

    [JsonPropertyName("body")]
    public MessageDto Body { get; set; }
}