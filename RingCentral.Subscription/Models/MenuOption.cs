namespace RingCentral.Subscription.Models;

/// <summary>
/// Enum representing the available menu options.
/// </summary>
public enum MenuOption
{
    /// <summary>
    /// Option to fetch a single extension.
    /// </summary>
    FetchExtension = 1,

    /// <summary>
    /// Option to fetch all extensions.
    /// </summary>
    FetchAllExtensions,

    /// <summary>
    /// Option to create a subscription.
    /// </summary>
    CreateSubscription,

    /// <summary>
    /// Option to create a subscription for extensions.
    /// </summary>
    CreateSubscriptionForExtensions,

    /// <summary>
    /// Option to fetch a subscription by its ID.
    /// </summary>
    FetchSubscription,

    /// <summary>
    /// Option to fetch all subscriptions.
    /// </summary>
    FetchAllSubscriptions,

    /// <summary>
    /// Option to add extensions to a subscription.
    /// </summary>
    AddExtensionsToSubscription,

    /// <summary>
    /// Options to remove extensions from a subscription.
    /// </summary>
    RemoveExtensionsFromSubscription,

    /// <summary>
    /// Option to delete a subscription by its ID.
    /// </summary>
    DeleteSubscription,

    /// <summary>
    /// Option to delete all subscriptions.
    /// </summary>
    DeleteAllSubscriptions,

    /// <summary>
    /// Option to exit the menu.
    /// </summary>
    Exit
}