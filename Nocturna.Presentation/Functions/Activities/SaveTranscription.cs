using Microsoft.Azure.Functions.Worker;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;

namespace Nocturna.Presentation.Functions.Activities;

public class SaveTranscription(IVoicemailProcessor processor)
{
    [Function(nameof(SaveTranscription))]
    public async Task Run(
        [ActivityTrigger] ActivityContext<TranscriptionInput> context,
        CancellationToken cancellationToken)
    {
        await processor.SaveVoicemailTranscriptionAsync(context, cancellationToken);
    }
}
