using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Application.Abstractions;

public interface IVoicemailProcessor
{
    Task<int> SavePayloadAsync(string payload, CancellationToken cancellationToken = default);
    Task<string> GetTranscriptionAsync(VoicemailMessage voicemailMsg, CancellationToken cancellationToken = default);
    Task SaveVoicemailTranscriptionAsync(WebhookPayloadDto payload, string transcription, int dbPayloadId, CancellationToken cancellationToken = default);
}