using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

public interface IVoicemailProcessor
{
    Task<int> SavePayloadAsync(ActivityContext<string> context, CancellationToken cancellationToken = default);
    Task<string> GetTranscriptionAsync(ActivityContext<VoicemailMessage> context, CancellationToken cancellationToken = default);
    Task SaveVoicemailTranscriptionAsync(ActivityContext<TranscriptionInput> context, CancellationToken cancellationToken = default);
}