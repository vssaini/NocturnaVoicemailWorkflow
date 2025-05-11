using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

public interface ITranscriptionWriter
{
    Task WriteTranscriptionToFtpAsync(WebhookPayloadDto payload, string transcription, CancellationToken cancellationToken = default);
}
