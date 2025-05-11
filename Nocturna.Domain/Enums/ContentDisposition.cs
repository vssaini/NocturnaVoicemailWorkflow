namespace Nocturna.Domain.Enums;

/// <summary>
/// Specifies how the message content should be handled by the client (browser or application).
/// </summary>
/// <remarks>
/// For more information, see the RingCentral API Reference:— <see href = "https://developers.ringcentral.com/api-reference/Message-Store/readMessageContent" />
/// </remarks>
public enum ContentDisposition
{
    /// <summary>
    /// Content is intended to be displayed inline in the browser.
    /// </summary>
    Inline,

    /// <summary>
    /// Content is intended to be downloaded and saved locally.
    /// </summary>
    Attachment
}
