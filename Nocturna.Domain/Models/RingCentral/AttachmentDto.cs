#nullable disable

using System.Text.Json.Serialization;

namespace Nocturna.Domain.Models.RingCentral;

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