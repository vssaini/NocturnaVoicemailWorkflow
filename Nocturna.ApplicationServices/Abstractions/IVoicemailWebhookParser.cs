using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

public interface IVoicemailWebhookParser
{
    WebhookPayloadDto ParsePayload(string body);
    bool IsValidPayload(WebhookPayloadDto payload);
    bool IsVoicemailEvent(WebhookPayloadDto payload);
    long? GetTranscriptionAttachmentId(MessageBodyDto message);
}