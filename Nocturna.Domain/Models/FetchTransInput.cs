using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Domain.Models;

public record FetchTransInput(WebhookPayloadDto Payload, long AttachmentId);