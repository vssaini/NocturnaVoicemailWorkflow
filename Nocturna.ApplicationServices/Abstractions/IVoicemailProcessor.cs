using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

public interface IVoicemailProcessor
{
    Task<int> SaveWebhookPayloadAsync(string payload, string requestUrl, CancellationToken cancellationToken = default);
    Task<string> GetTranscriptionAsync(long messageId, long attachmentId, CancellationToken cancellationToken = default);
    Task SaveVoicemailTranscriptionAsync(WebhookPayloadDto payload, string transcription, int dbPayloadId, CancellationToken cancellationToken = default);
}