using Nocturna.Domain.Models.RingCentral;
using Refit;

namespace Nocturna.Infrastructure.RingCentral.Clients;

public interface IRingCentralMediaApi
{
    [Get("/restapi/v1.0/account/~/extension/~/message-store/{messageId}/content/{attachmentId}")]
    Task<string> GetMessageAttachmentContent(
        long messageId,
        long attachmentId,
        [Query] string contentDisposition,
        CancellationToken cancellationToken = default);

    [Get("/restapi/v1.0/account/~/extension/~/message-store/{messageId}")]
    Task<MessageDto> GetMessage(
        long messageId,
        CancellationToken cancellationToken = default);
}