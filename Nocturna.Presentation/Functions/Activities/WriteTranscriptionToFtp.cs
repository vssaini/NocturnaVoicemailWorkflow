using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;

namespace Nocturna.Presentation.Functions.Activities;

public class WriteTranscriptionToFtp(ITranscriptionWriter transWriter)
{
    [FunctionName(nameof(WriteTranscriptionToFtp))]
    public async Task Run(
        [ActivityTrigger] TranscriptionInput input,
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        await transWriter.WriteTranscriptionToFtpAsync(input.Payload, input.Transcription, cancellationToken);
    }
}
