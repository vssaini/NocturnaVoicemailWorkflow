namespace Nocturna.Domain.Models;

public class WebhookInput
{
    public string? Payload { get; set; } = string.Empty;
    public Dictionary<string, IEnumerable<string>> Headers { get; set; } = new();
}
