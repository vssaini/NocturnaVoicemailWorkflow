using Microsoft.Azure.Functions.Worker;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;

namespace Nocturna.Presentation.Functions.Activities;

public class SaveTranscription(IVoicemailProcessor processor)
{
    [Function(nameof(SaveTranscription))]
    public async Task Run(
        [ActivityTrigger] TranscriptionInput input,
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        await processor.SaveVoicemailTranscriptionAsync(input.Payload, input.Transcription, input.DbPayloadId, cancellationToken);
    }
}
