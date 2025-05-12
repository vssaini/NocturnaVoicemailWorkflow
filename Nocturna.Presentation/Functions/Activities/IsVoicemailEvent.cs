using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Presentation.Functions.Activities;

public class IsVoicemailEvent(IVoicemailWebhookParser parser)
{
    [Function(nameof(IsVoicemailEvent))]
    public bool Run([ActivityTrigger] WebhookPayloadDto payloadDto,
        FunctionContext context)
    {
        var logger = context.GetLogger(nameof(IsVoicemailEvent));

        if (parser.IsVoicemailEvent(payloadDto))
            return true;

        logger.LogWarning("Unsupported event type: {Event}. Only 'voicemail' events are supported.", payloadDto.Event);
        return false;
    }
}