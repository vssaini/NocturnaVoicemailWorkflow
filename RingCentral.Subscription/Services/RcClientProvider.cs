using RingCentral.Subscription.Helpers;

namespace RingCentral.Subscription.Services;

/// <summary>
/// Provides services to interact with the RingCentral API, including initialization
/// of the client and authorization.
/// </summary>
public class RcClientProvider
{
    private const string YellowColor = "\u001b[33m";
    private const string BlueColor = "\u001b[38;2;0;123;255m";
    private const string OrangeColor = "\u001b[38;2;255;165;0m";
    private const string DefaultColor = "\u001b[0m";

    /// <summary>
    /// Gets the RestClient instance used to communicate with the RingCentral API.
    /// </summary>
    public RestClient? Client { get; private set; }

    /// <summary>
    /// Gets the delivery address for the webhook, constructed from the environment variable.
    /// </summary>
    public string? DeliveryAddress { get; private set; }
    
    /// <summary>
    /// Initializes the application by retrieving required environment variables, 
    /// validating them, authorizing with the RingCentral platform, and displaying the authorization status.
    /// </summary>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when one or more required environment variables are missing or empty.
    /// </exception>
    public async Task InitializeAsync()
    {
        ConsolePrinter.Info("Authorizing and fetching token...");

        var webhookDeliveryAddress = GetEnvironmentVariable("WEBHOOK_DELIVERY_ADDRESS");
        var appClientId = GetEnvironmentVariable("RC_APP_CLIENT_ID");
        var appClientSecret = GetEnvironmentVariable("RC_APP_CLIENT_SECRET");
        var rcServerUrl = GetEnvironmentVariable("RC_SERVER_URL");
        var userJwt = GetEnvironmentVariable("RC_USER_JWT");

        ValidateEnvironmentVariable(webhookDeliveryAddress, "WEBHOOK_DELIVERY_ADDRESS");
        ValidateEnvironmentVariable(appClientId, "RC_APP_CLIENT_ID");
        ValidateEnvironmentVariable(appClientSecret, "RC_APP_CLIENT_SECRET");
        ValidateEnvironmentVariable(rcServerUrl, "RC_SERVER_URL");
        ValidateEnvironmentVariable(userJwt, "RC_USER_JWT");

        DeliveryAddress = webhookDeliveryAddress;

        Client = new RestClient(appClientId, appClientSecret, rcServerUrl);
        var tokenInfo = await Client.Authorize(userJwt);
        var authorizationMessage = tokenInfo != null ? "Authorization successful." : "Authorization failed.";

        DisplayAuthorizationStatus(webhookDeliveryAddress, appClientId, rcServerUrl, authorizationMessage, YellowColor);
        Console.Write(Environment.NewLine);
    }

    private static string? GetEnvironmentVariable(string variableName)
    {
        return Environment.GetEnvironmentVariable(variableName);
    }

    private static void ValidateEnvironmentVariable(string? value, string variableName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Environment variable '{variableName}' is missing or empty.");
    }

    private static void DisplayAuthorizationStatus(string? deliveryAddress, string? clientId, string? serverUrl, string authMessage, string yellow)
    {
        ConsoleHelper.EnableVirtualTerminalProcessing();

        var lines = new[]
        {
        $"{BlueColor}Authorization Status{DefaultColor}",
        $"{OrangeColor}Webhook delivery address:{DefaultColor} {deliveryAddress}",
        $"{OrangeColor}Client ID:{DefaultColor} {clientId}",
        $"{OrangeColor}Server URL:{DefaultColor} {serverUrl}",
        $"{authMessage}"
    };

        var boxWidth = lines.Max(l => StripAnsi(l).Length) + 4;
        var top = $"{yellow}┌{new string('─', boxWidth - 2)}┐{DefaultColor}";
        var bottom = $"{yellow}└{new string('─', boxWidth - 2)}┘{DefaultColor}";

        Console.WriteLine(top);
        foreach (var line in lines)
        {
            Console.WriteLine($"{yellow}│ {DefaultColor}{line.PadRight(boxWidth - 4 + GetAnsiPadding(line))}{yellow} │{DefaultColor}");
        }
        Console.WriteLine(bottom);
    }

    private static string StripAnsi(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, @"\x1B\[[0-9;]*m", "");
    }

    private static int GetAnsiPadding(string input)
    {
        return input.Length - StripAnsi(input).Length;
    }
}
