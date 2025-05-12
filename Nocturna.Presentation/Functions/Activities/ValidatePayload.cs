using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;

namespace Nocturna.Presentation.Functions.Activities;

public class ValidatePayload(IVoicemailWebhookParser parser)
{
    [Function(nameof(ValidatePayload))]
    public bool Run([ActivityTrigger] string payload,
        FunctionContext context)
    {
        var logger = context.GetLogger(nameof(ValidatePayload));

        var payloadDto = parser.ParsePayload(payload);
        if (parser.IsValidPayload(payloadDto)) 
            return true;

        logger.LogWarning("Invalid payload: {@Payload}", payloadDto);
        return false;
    }
}