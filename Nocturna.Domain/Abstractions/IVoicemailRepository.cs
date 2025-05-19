using Nocturna.Domain.Entities;
using Nocturna.Domain.Models;

namespace Nocturna.Domain.Abstractions;

public interface IVoicemailRepository
{
    Task<int> SaveWebhookPayloadAsync(ActivityContext<string> context, CancellationToken cancellationToken = default);
    Task SaveVoicemailTranscriptionAsync(int savedPayloadId, ActivityContext<VoicemailTranscription> context, CancellationToken cancellationToken = default);
}