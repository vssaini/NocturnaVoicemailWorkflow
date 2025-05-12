#nullable disable

using System.Text.Json.Serialization;

namespace Nocturna.Domain.Models.RingCentral;

public class MessageDto
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