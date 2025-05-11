using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using System.Text.Json;
using Nocturna.Domain.Models;

namespace Nocturna.Infrastructure.Services;

public class VoicemailWebhookParser(ILogger<VoicemailWebhookParser> logger) : IVoicemailWebhookParser
{
    public WebhookPayloadDto? ParsePayload(string body)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                return JsonSerializer.Deserialize<WebhookPayloadDto>(body);
            }
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid JSON in webhook payload.");
        }

        return null;
    }

    public bool IsValidPayload(WebhookPayloadDto? payload)
    {
        return payload != null && !string.IsNullOrWhiteSpace(payload.Event);
    }

    public bool IsVoicemailEvent(WebhookPayloadDto payload)
    {
        var eventPath = payload.Event;
        return eventPath.StartsWith("/restapi/v1.0/account/") &&
               eventPath.Contains("/extension/") &&
               eventPath.EndsWith("/voicemail");
    }

    public long? GetTranscriptionAttachmentId(MessageBodyDto message)
    {
        return message.Attachments.FirstOrDefault(a => a.Type == "AudioTranscription")?.Id;
    }
}
