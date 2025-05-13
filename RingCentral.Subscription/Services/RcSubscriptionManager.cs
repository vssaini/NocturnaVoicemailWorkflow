using Microsoft.Extensions.Options;
using RingCentral.Subscription.Helpers;
using RingCentral.Subscription.Models;
using System.Text;

namespace RingCentral.Subscription.Services;

/// <summary>
/// Manages RingCentral subscription-related operations such as creating, fetching, and deleting subscriptions,
/// as well as retrieving extension details.
/// </summary>
public class RcSubscriptionManager(RcClientProvider rcProvider, IOptions<SubscriptionSetting> options)
{
    private readonly SubscriptionSetting _subscription = options.Value;
    private readonly RestClient _client = rcProvider.Client!;

    /// <summary>
    /// Fetches a specific extension by ID and displays its details if it is enabled and of type 'User'.
    /// </summary>
    /// <param name="extensionId">The ID of the extension to fetch.</param>
    public async Task FetchExtensionAsync(string extensionId)
    {
        try
        {
            var response = await _client.Restapi().Account().Extension(extensionId).Get();

            if (response.status != "Enabled" || response.type != "User")
            {
                ConsolePrinter.Warning($"Extension {extensionId} is not a user-enabled extension.");
                return;
            }

            ConsolePrinter.Info($"Extension ID: {response.id}");
            ConsolePrinter.Info($"Name: {response.name}");
            ConsolePrinter.Info($"Email: {response.contact?.email}");
            ConsolePrinter.Info($"Type: {response.type}");
            ConsolePrinter.Info($"Status: {response.status}");
            ConsolePrinter.Info($"Extension Number: {response.extensionNumber}");
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error($"Failed to fetch extension {extensionId}: {ex.Message}");
        }
    }

    /// <summary>
    /// Fetches all extensions and displays the ones that are enabled user extensions.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    public async Task FetchAllExtensionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.Restapi().Account().Extension().List();
            var totalExtensions = response.records.Length;
            if (totalExtensions <= 0)
            {
                ConsolePrinter.Warning(Constants.Messages.NoSubExist);
                return;
            }

            var page = response.paging.page;
            var pageCount = response.paging.totalPages;

            var extensions = response.records
                .OrderBy(r => r.name)
                .Where(r => r.status == "Enabled" && r.type == "User")
                .ToList();

            var userAndEnabledExtensions = extensions.Count;

            ConsolePrinter.Info($"Found {totalExtensions} extensions (Page {page}/{pageCount}), {userAndEnabledExtensions} user-enabled.{Environment.NewLine}");
            ConsolePrinter.PrintExtensionsTable(extensions);
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to fetch all extensions: " + ex.Message);
        }
    }

    /// <summary>
    /// Creates a voicemail notification subscription for all user-enabled extensions.
    /// </summary>
    public async Task CreateVoicemailSubscriptionAsync()
    {
        try
        {
            const string voiceMailFilter = "/restapi/v1.0/account/~/extension/~/voicemail";
            var voicemailFilters = new[] { voiceMailFilter };
            var request = BuildSubscriptionRequestForVoiceMail(voicemailFilters);
            var response = await _client.Restapi().Subscription().Post(request);

            ConsolePrinter.Success("Subscription created successfully.");
            LogSubscriptionResponse(response);
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to create subscription: " + ex.Message);
        }
    }

    /// <summary>
    /// Creates voicemail notification subscription for a specific set of extension IDs.
    /// </summary>
    /// <param name="extensionIds">Array of extension IDs to subscribe to voicemail events.</param>
    public async Task CreateVoicemailSubscriptionAsync(int[] extensionIds)
    {
        try
        {
            var voicemailFilters = extensionIds
                .Select(extensionId => $"/restapi/v1.0/account/~/extension/{extensionId}/voicemail")
                .ToList();

            var request = BuildSubscriptionRequestForVoiceMail(voicemailFilters.ToArray());
            var response = await _client.Restapi().Subscription().Post(request);

            ConsolePrinter.Success("Subscription for extensions created successfully.");
            LogSubscriptionResponse(response);
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to create subscription for extensions: " + ex.Message);
        }
    }

    /// <summary>
    /// Fetches and logs details of a specific subscription by its ID.
    /// </summary>
    /// <param name="subscriptionId">The ID of the subscription to retrieve.</param>
    public async Task FetchSubscriptionAsync(string subscriptionId)
    {
        try
        {
            var subscription = await _client.Restapi().Subscription(subscriptionId).Get();
            LogSubscriptionResponse(subscription);
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to fetch subscription: " + ex.Message);
        }
    }

    /// <summary>
    /// Fetches and logs all current subscriptions associated with the account.
    /// </summary>
    public async Task FetchAllSubscriptionsAsync()
    {
        try
        {
            var response = await _client.Restapi().Subscription().List();
            if (response.records.Length == 0)
            {
                ConsolePrinter.Warning(Constants.Messages.NoSubExist);
                return;
            }

            foreach (var subInfo in response.records)
            {
                LogSubscriptionResponse(subInfo);
                Console.Write(Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to read subscriptions: " + ex.Message);
        }
    }

    /// <summary>
    /// Deletes a specific subscription by its ID.
    /// </summary>
    /// <param name="subscriptionId">The ID of the subscription to delete.</param>
    public async Task DeleteSubscriptionAsync(string subscriptionId)
    {
        try
        {
            await _client.Restapi().Subscription(subscriptionId).Delete();
            ConsolePrinter.Success("Subscription " + subscriptionId + " deleted.");
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to delete subscription: " + ex.Message);
        }
    }

    /// <summary>
    /// Deletes all current subscriptions from the account.
    /// </summary>
    public async Task DeleteAllSubscriptionsAsync()
    {
        try
        {
            var response = await _client.Restapi().Subscription().List();
            if (response.records.Length == 0)
            {
                ConsolePrinter.Warning(Constants.Messages.NoSubExist);
                return;
            }

            foreach (var subInfo in response.records)
            {
                LogSubscriptionResponse(subInfo);
                await DeleteSubscriptionAsync(subInfo.id);
                Console.Write(Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to delete all subscriptions: " + ex.Message);
        }
    }

    // Helpers

    /// <summary>
    /// Logs detailed information about a subscription.
    /// </summary>
    /// <param name="response">The subscription response object to log.</param>
    private static void LogSubscriptionResponse(SubscriptionInfo response)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Subscription ID: {response.id}, Subscription URI: {response.uri}, Event Filters: {string.Join(", ", response.eventFilters)}");
        sb.AppendLine($"Expiration Time: {response.expirationTime}, Expires In: {response.expiresIn} seconds");
        sb.AppendLine($"Subscription Status: {response.status}, Creation Time: {response.creationTime}");

        if (response.deliveryMode != null)
        {
            sb.AppendLine("Webhook Delivery Mode:");
            sb.AppendLine($" Transport Type: {response.deliveryMode.transportType}");
            sb.AppendLine($" Address: {response.deliveryMode.address}");
            if (!string.IsNullOrWhiteSpace(response.deliveryMode.secretKey))
                sb.AppendLine($" • Secret Key: {response.deliveryMode.secretKey}");
        }

        ConsolePrinter.Info(sb.ToString());
    }

    /// <summary>
    /// Builds a CreateSubscriptionRequest object for voicemail filters.
    /// </summary>
    /// <param name="voicemailFilters">Array of voicemail event filters.</param>
    /// <returns>A populated CreateSubscriptionRequest object.</returns>
    private CreateSubscriptionRequest BuildSubscriptionRequestForVoiceMail(string[] voicemailFilters)
    {
        var verificationToken = TokenGenerator.GenerateVerificationToken(_subscription.Environment);
        ConsolePrinter.Info("Verification token for Azure Function (WebhookSecret): " + verificationToken);

        return new CreateSubscriptionRequest
        {
            eventFilters = voicemailFilters,
            deliveryMode = new NotificationDeliveryModeRequest
            {
                transportType = "WebHook",
                address = rcProvider.DeliveryAddress,
                verificationToken = verificationToken
            },
            expiresIn = _subscription.ExpiresInSeconds
        };
    }
}
