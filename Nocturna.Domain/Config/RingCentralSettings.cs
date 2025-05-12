using System.ComponentModel.DataAnnotations;

namespace Nocturna.Domain.Config;

/// <summary>
/// Configuration settings for connecting to RingCentral services.
/// </summary>
public class RingCentralSettings
{
    /// <summary>
    /// The base URL of the RingCentral server.
    /// </summary>
    [Required(ErrorMessage = "RingCentral's ServerUrl is required.")]
    public string ServerUrl { get; init; } = "https://platform.ringcentral.com"; // Auth

    /// <summary>
    /// The URL used to access RingCentral media files.
    /// </summary>
    [Required(ErrorMessage = "RingCentral's MediaUrl is required.")]
    public string MediaUrl { get; init; } = "https://media.ringcentral.com";     // File content

    /// <summary>
    /// The client ID for RingCentral OAuth authentication.
    /// </summary>
    [Required(ErrorMessage = "ClientId is required.")]
    public required string ClientId { get; init; }

    /// <summary>
    /// The client secret for RingCentral OAuth authentication.
    /// </summary>
    [Required(ErrorMessage = "ClientSecret is required.")]
    public required string ClientSecret { get; init; }

    /// <summary>
    /// The JWT token used for RingCentral API access.
    /// </summary>
    [Required(ErrorMessage = "JwtToken is required.")]
    public required string JwtToken { get; init; }

    /// <summary>
    /// The RingCentral verification token for subscription.
    /// </summary>
    [Required(ErrorMessage = "WebhookSecret is required.")]
    public required string WebhookSecret { get; init; }
}

