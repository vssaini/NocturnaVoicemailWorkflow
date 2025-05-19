using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

public interface ITranscriptionWriter
{
    Task WriteTranscriptionToFtpAsync(ActivityContext<TranscriptionInput> context, CancellationToken cancellationToken = default);
}
