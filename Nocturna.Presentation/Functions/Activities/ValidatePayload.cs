using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Presentation.Functions.Activities;

public class ValidatePayload(IVoicemailWebhookParser parser)
{
    [Function(nameof(ValidatePayload))]
    public bool Run([ActivityTrigger] WebhookPayloadDto payloadDto,
        FunctionContext context)
    {
        var logger = context.GetLogger(nameof(ValidatePayload));

        if (parser.IsValidPayload(payloadDto))
            return true;

        logger.LogWarning("Invalid payload: {@Payload}", payloadDto);
        return false;
    }
}