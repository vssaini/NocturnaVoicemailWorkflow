using System.Text.Json.Serialization;

namespace Nocturna.Domain.Models;

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
    public MessageBodyDto Body { get; set; }
}

public class MessageBodyDto
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("to")]
    public ToDto[] To { get; set; }

    [JsonPropertyName("from")]
    public FromDto From { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("creationTime")]
    public DateTime CreationTime { get; set; }

    [JsonPropertyName("readStatus")]
    public string ReadStatus { get; set; }

    [JsonPropertyName("priority")]
    public string Priority { get; set; }

    [JsonPropertyName("attachments")]
    public AttachmentDto[] Attachments { get; set; }

    [JsonPropertyName("direction")]
    public string Direction { get; set; }

    [JsonPropertyName("availability")]
    public string Availability { get; set; }

    [JsonPropertyName("messageStatus")]
    public string MessageStatus { get; set; }

    [JsonPropertyName("lastModifiedTime")]
    public DateTime LastModifiedTime { get; set; }

    [JsonPropertyName("vmTranscriptionStatus")]
    public string VmTranscriptionStatus { get; set; }

    [JsonPropertyName("eventType")]
    public string EventType { get; set; }
}

public class ToDto
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }
}

public class FromDto
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("phoneNumberInfo")]
    public PhoneNumberInfoDto PhoneNumberInfo { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class PhoneNumberInfoDto
{
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; }

    [JsonPropertyName("nationalDestinationCode")]
    public string NationalDestinationCode { get; set; }

    [JsonPropertyName("subscriberNumber")]
    public string SubscriberNumber { get; set; }
}

public class AttachmentDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; }

    [JsonPropertyName("vmDuration")]
    public int VmDuration { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; }
}