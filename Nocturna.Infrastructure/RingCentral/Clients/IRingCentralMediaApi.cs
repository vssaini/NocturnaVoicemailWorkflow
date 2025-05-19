using Refit;

namespace Nocturna.Infrastructure.RingCentral.Clients;

public interface IRingCentralMediaApi
{
    /// <summary>
    /// Retrieves the content of an attachment in a message from the specified account and extension.
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <param name="extensionId">The ID of the extension.</param>
    /// <param name="messageId">The ID of the message.</param>
    /// <param name="attachmentId">The ID of the attachment.</param>
    /// <param name="contentDisposition">The content disposition of the attachment.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, with the content of the attachment as a string.</returns>
    [Get("/restapi/v1.0/account/{accountId}/extension/{extensionId}/message-store/{messageId}/content/{attachmentId}")]
    Task<string> GetMessageAttachmentContentAsync(long accountId,
        long extensionId,
        long messageId,
        long attachmentId,
        [Query] string contentDisposition,
        CancellationToken cancellationToken = default);
}
