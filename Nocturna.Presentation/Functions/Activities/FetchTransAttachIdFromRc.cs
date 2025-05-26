using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Enums;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Presentation.Helpers;

namespace Nocturna.Presentation.Functions.Activities;

public class FetchTransAttachIdFromRc(IMessageFetcher messageFetcher, IVoicemailWebhookParser parser)
{
    [Function(nameof(FetchTransAttachIdFromRc))]
    public async Task<TranscriptionResult> Run(
        [ActivityTrigger] ActivityContext<WebhookPayloadDto> context,
        FunctionContext funcContext,
        CancellationToken cancellationToken)
    {
        var logger = funcContext.GetLogger<FetchTransAttachIdFromRc>();

        try
        {
            var (accountId, extensionId) = EventPathParser.ParseAccountAndExtensionFromPath(context.Data.Event);

            var msgRequest = new MessageRequest(accountId, extensionId, context.Data.Body.Id);
            var msgContext = new ActivityContext<MessageRequest>(context.PayloadUuid, msgRequest);

            var message = await messageFetcher.GetMessageAsync(msgContext, cancellationToken);
            if (message == null)
                return new TranscriptionResult(null, VmTranscriptionStatus.Unknown);

            var attachmentId = parser.GetTranscriptionAttachmentId(message);
            return new TranscriptionResult(attachmentId, message.VmTranscriptionStatus);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Payload {PayloadUuid} - Error '{ErrorMessage}' while fetching attachment Id from RC", context.Data.Uuid, ex.Message);
            return new TranscriptionResult(null, VmTranscriptionStatus.Unknown);
        }
    }
}
