using Microsoft.Azure.Functions.Worker;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Presentation.Functions.Activities;

public class FetchTransAttachId(IVoicemailWebhookParser parser)
{
    [Function(nameof(FetchTransAttachId))]
    public long? Run(
        [ActivityTrigger] WebhookPayloadDto payloadDto)
    {
       var message = payloadDto.Body;
        var transcriptionId = parser.GetTranscriptionAttachmentId(message);
        return transcriptionId ?? null;
    }
}
