using System.Text;

namespace RingCentral.Subscription.Helpers;

/// <summary>
/// Generates a verification token compliant with RingCentral requirements.
/// Only alphanumeric characters allowed, no special symbols or dashes.
/// </summary>
public class TokenGenerator
{
    /// <summary>
    /// Generates a secure, alphanumeric verification token for RingCentral webhook validation.
    /// </summary>
    /// <param name="environment">Environment name such as "prod" or "dev". Used as a prefix.</param>
    /// <returns>An alphanumeric string token.</returns>
    public static string GenerateVerificationToken(string environment)
    {
        if (string.IsNullOrEmpty(environment))
            environment = "prod";

        const string company = "noc";
        const string service = "rc";

        var randomString = GenerateShortToken();

        var rawToken = $"{environment}-{company}-{service}-{randomString}";
        return rawToken;
    }

    public static string GenerateShortToken(int length = 5)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var token = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            token.Append(chars[random.Next(chars.Length)]);
        }

        return token.ToString();
    }
}