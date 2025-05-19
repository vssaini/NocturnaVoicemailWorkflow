#nullable enable
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Application.Abstractions;

public interface IVoicemailWebhookParser
{
    WebhookPayloadDto? ParsePayload(string body);
    long? GetTranscriptionAttachmentId(MessageDto? message);
}