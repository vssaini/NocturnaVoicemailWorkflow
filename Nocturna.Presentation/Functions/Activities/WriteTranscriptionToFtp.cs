using Microsoft.Azure.Functions.Worker;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;

namespace Nocturna.Presentation.Functions.Activities;

public class WriteTranscriptionToFtp(ITranscriptionWriter transWriter)
{
    [Function(nameof(WriteTranscriptionToFtp))]
    public async Task Run(
        [ActivityTrigger] TranscriptionInput input,
        CancellationToken cancellationToken)
    {
        await transWriter.WriteTranscriptionToFtpAsync(input.Payload, input.Transcription, cancellationToken);
    }
}
