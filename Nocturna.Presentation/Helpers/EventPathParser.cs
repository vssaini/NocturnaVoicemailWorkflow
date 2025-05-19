using System.Text.RegularExpressions;

namespace Nocturna.Presentation.Helpers;

/// <summary>
/// Provides functionality for parsing event paths to extract account and extension id.
/// </summary>
public class EventPathParser
{
    /// <summary>
    /// Parses the account ID and extension ID from the provided event path.
    /// </summary>
    /// <param name="eventPath">The event path string containing the account and extension information.</param>
    /// <returns>
    /// A tuple containing the account ID and extension ID as long values.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the event path format is invalid or parsing fails.
    /// </exception>
    public static (long accountId, long extensionId) ParseAccountAndExtensionFromPath(string eventPath)
    {
        var match = Regex.Match(eventPath, @"account/(?<accountId>\d+)/extension/(?<extensionId>\d+)");
        if (!match.Success ||
            !long.TryParse(match.Groups["accountId"].Value, out var accountId) ||
            !long.TryParse(match.Groups["extensionId"].Value, out var extensionId))
        {
            throw new InvalidOperationException("Invalid event format");
        }

        return (accountId, extensionId);
    }
}
