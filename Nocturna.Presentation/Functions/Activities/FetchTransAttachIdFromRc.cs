using Microsoft.Azure.Functions.Worker;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Presentation.Helpers;

namespace Nocturna.Presentation.Functions.Activities;

public class FetchTransAttachIdFromRc(IMessageFetcher messageFetcher, IVoicemailWebhookParser parser)
{
    [Function(nameof(FetchTransAttachIdFromRc))]
    public async Task<long?> Run(
        [ActivityTrigger] ActivityContext<WebhookPayloadDto> context,
        CancellationToken cancellationToken)
    {
        var (accountId, extensionId) = EventPathParser.ParseAccountAndExtensionFromPath(context.Data.Event);

        var msgRequest = new MessageRequest(accountId, extensionId, context.Data.Body.Id);
        var msgContext = new ActivityContext<MessageRequest>(context.PayloadUuid, msgRequest);

        var message = await messageFetcher.GetMessageAsync(msgContext, cancellationToken);
        return parser.GetTranscriptionAttachmentId(message);
    }
}
