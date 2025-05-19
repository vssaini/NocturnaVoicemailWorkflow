using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models.RingCentral;
using System.Text.Json;

namespace Nocturna.Infrastructure.Services;

public class VoicemailWebhookParser(ILogger<VoicemailWebhookParser> logger) : IVoicemailWebhookParser
{
    public WebhookPayloadDto? ParsePayload(string body)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(body))
                return null;

            var payload = JsonSerializer.Deserialize<WebhookPayloadDto>(body);
            if (!IsValidPayload(payload))
                return null;

            if (IsVoicemailEvent(payload!))
                return payload;

            logger.LogWarning("Unsupported event type: {Event}. Only 'voicemail' events are supported.", payload!.Event);
            return null;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid JSON in webhook payload.");
        }

        return null;
    }

    private static bool IsValidPayload(WebhookPayloadDto? payload)
    {
        return payload != null && !string.IsNullOrWhiteSpace(payload.Event);
    }

    private static bool IsVoicemailEvent(WebhookPayloadDto payload)
    {
        var eventPath = payload.Event;
        return eventPath.StartsWith("/restapi/v1.0/account/") &&
               eventPath.Contains("/extension/") &&
               eventPath.EndsWith("/voicemail");
    }

    public long? GetTranscriptionAttachmentId(MessageDto? message)
    {
        return message?.Attachments?.FirstOrDefault(a => a.Type == "AudioTranscription")?.Id;
    }
}
