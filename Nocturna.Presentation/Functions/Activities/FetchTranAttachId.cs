using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Presentation.Functions.Activities;

public class FetchTranAttachId(IVoicemailWebhookParser parser)
{
    [FunctionName(nameof(FetchTranAttachId))]
    public long? Run(
        [ActivityTrigger] WebhookPayloadDto payloadDto,
        FunctionContext context)
    {
        var logger = context.GetLogger(nameof(FetchTranAttachId));

        var message = payloadDto.Body;
        var transcriptionId = parser.GetTranscriptionAttachmentId(message);
        return transcriptionId ?? null;
    }
}
