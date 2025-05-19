using Microsoft.Azure.Functions.Worker;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Presentation.Helpers;

namespace Nocturna.Presentation.Functions.Activities;

public class FetchTranscription(IVoicemailProcessor processor)
{
    [Function(nameof(FetchTranscription))]
    public async Task<string?> Run(
        [ActivityTrigger] ActivityContext<FetchTransInput> context,
        CancellationToken cancellationToken)
    {
        var (accountId, extensionId) = EventPathParser.ParseAccountAndExtensionFromPath(context.Data.Payload.Event);

        var voicemailMsg = new VoicemailMessage(accountId, extensionId, context.Data.Payload.Body.Id, context.Data.AttachmentId);
        var voicemailContext = new ActivityContext<VoicemailMessage>(context.PayloadUuid,voicemailMsg);

        var transcription = await processor.GetTranscriptionAsync(voicemailContext, cancellationToken);
        return !string.IsNullOrWhiteSpace(transcription) ? transcription : null;
    }
}
