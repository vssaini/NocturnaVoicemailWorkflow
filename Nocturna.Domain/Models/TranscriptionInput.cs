using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Domain.Models;

public record TranscriptionInput(WebhookPayloadDto Payload, string Transcription, int SavedPayloadId);
