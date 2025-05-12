using Nocturna.Domain.Models.RingCentral;
using Refit;

namespace Nocturna.Infrastructure.RingCentral.Clients;

public interface IRingCentralMediaApi
{
    [Get("/restapi/v1.0/account/~/extension/~/message-store/{messageId}/content/{attachmentId}")]
    Task<string> GetMessageAttachmentContentAsync(
        long messageId,
        long attachmentId,
        [Query] string contentDisposition,
        CancellationToken cancellationToken = default);

    [Get("/restapi/v1.0/account/~/extension/~/message-store/{messageId}")]
    Task<MessageDto> GetMessageAsync(
        long messageId,
        CancellationToken cancellationToken = default);
}