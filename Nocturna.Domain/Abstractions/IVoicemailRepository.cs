using Nocturna.Domain.Entities;

namespace Nocturna.Domain.Abstractions;

public interface IVoicemailRepository
{
    Task<int> SaveWebhookPayloadAsync(string payload, CancellationToken cancellationToken = default);
    Task SaveVoicemailTranscriptionAsync(int payloadId, VoicemailTranscription transcription, CancellationToken cancellationToken = default);
}