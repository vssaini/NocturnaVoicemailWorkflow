using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;

namespace Nocturna.Presentation.Functions.Activities;

public class FetchTranscription(IVoicemailProcessor processor)
{
    [Function(nameof(FetchTranscription))]
    public async Task<string?> Run(
        [ActivityTrigger] VoicemailMessage voicemailMsg,
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        var logger = context.GetLogger(nameof(FetchTranscription));

        var transcription = await processor.GetTranscriptionAsync(voicemailMsg, cancellationToken);
        if (!string.IsNullOrWhiteSpace(transcription))
            return transcription;

        logger.LogWarning("Transcription is empty or null for message {MessageId} (attachment {AttachmentId}).", voicemailMsg.MessageId, voicemailMsg.AttachmentId);
        return null;
    }
}
