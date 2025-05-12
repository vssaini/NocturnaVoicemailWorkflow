using Microsoft.Azure.Functions.Worker;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Presentation.Functions.Activities;

public class FetchTransAttachIdFromRc(IMessageFetcher messageFetcher, IVoicemailWebhookParser parser)
{
    [Function(nameof(FetchTransAttachIdFromRc))]
    public async Task<long?> Run(
        [ActivityTrigger] WebhookPayloadDto payloadDto)
    {
        var message = await messageFetcher.GetMessageAsync(new MessageRequest(payloadDto.Body.Id));
        return parser.GetTranscriptionAttachmentId(message);
    }
}
