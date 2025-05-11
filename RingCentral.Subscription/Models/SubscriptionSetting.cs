namespace RingCentral.Subscription.Models;

/// <summary>
/// Represents configuration settings for RingCentral webhook subscriptions,
/// bound from the <c>RingCentral:Subscription</c> section in <c>appsettings.json</c>.
/// </summary>
public class SubscriptionSetting
{
    /// <summary>
    /// Gets the environment label (e.g., "dev", "prod") used for verification token.
    /// </summary>
    public required string Environment { get; init; }

    /// <summary>
    /// Gets the subscription expiration duration in seconds.
    /// </summary>
    public int ExpiresInSeconds { get; init; }
}
