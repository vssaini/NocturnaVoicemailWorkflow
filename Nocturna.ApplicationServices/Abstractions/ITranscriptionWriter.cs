using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Application.Abstractions;

public interface ITranscriptionWriter
{
    Task WriteTranscriptionToFtpAsync(WebhookPayloadDto payload, string transcription, CancellationToken cancellationToken = default);
}
