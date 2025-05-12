using RingCentral.Subscription.Helpers;

namespace RingCentral.Subscription.Services;

/// <summary>
/// Provides services to interact with the RingCentral API, including initialization
/// of the client and authorization.
/// </summary>
public class RcClientProvider
{
    /// <summary>
    /// Gets the RestClient instance used to communicate with the RingCentral API.
    /// </summary>
    public RestClient? Client { get; private set; }

    /// <summary>
    /// Gets the delivery address for the webhook, constructed from the environment variable.
    /// </summary>
    public string? DeliveryAddress { get; private set; }

    /// <summary>
    /// Initializes the RingCentralService by setting up the RestClient and
    /// authorizing using environment variables.
    /// </summary>
    /// <exception cref="Exception">Thrown when authorization fails.</exception>
    public async Task InitializeAsync()
    {
        DeliveryAddress = Environment.GetEnvironmentVariable("WEBHOOK_DELIVERY_ADDRESS");
        ConsolePrinter.Info("Webhook delivery address: " + DeliveryAddress);
        Console.Write(Environment.NewLine);

        Client = new RestClient(
            Environment.GetEnvironmentVariable("RC_APP_CLIENT_ID"),
            Environment.GetEnvironmentVariable("RC_APP_CLIENT_SECRET"),
            Environment.GetEnvironmentVariable("RC_SERVER_URL"));

        await Client.Authorize(Environment.GetEnvironmentVariable("RC_USER_JWT"));
    }
}
