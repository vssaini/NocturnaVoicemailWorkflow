using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Presentation.Functions.Activities;

public class FetchTranAttachIdFromRingCentral(IMessageFetcher messageFetcher, IVoicemailWebhookParser parser)
{
    [FunctionName(nameof(FetchTranAttachIdFromRingCentral))]
    public async Task<long?> Run(
        [ActivityTrigger] WebhookPayloadDto payloadDto,
        FunctionContext context)
    {
        var message = await messageFetcher.GetMessageAsync(new MessageRequest(payloadDto.Body.Id));
        return parser.GetTranscriptionAttachmentId(message);
    }
}
