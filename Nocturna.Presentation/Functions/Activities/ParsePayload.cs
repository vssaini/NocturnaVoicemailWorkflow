using Microsoft.Azure.Functions.Worker;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Presentation.Functions.Activities;

public class ParsePayload(IVoicemailWebhookParser parser)
{
    [Function(nameof(ParsePayload))]
    public WebhookPayloadDto? Run(
        [ActivityTrigger] string payload)
    {
        var payloadDto = parser.ParsePayload(payload);
        return payloadDto;
    }
}
