using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Application.Abstractions;

public interface IVoicemailWebhookParser
{
    WebhookPayloadDto ParsePayload(string body);
    bool IsValidPayload(WebhookPayloadDto payload);
    bool IsVoicemailEvent(WebhookPayloadDto payload);
    long? GetTranscriptionAttachmentId(MessageDto message);
}