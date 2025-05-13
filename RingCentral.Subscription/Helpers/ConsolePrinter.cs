using RingCentral.Subscription.Models;

namespace RingCentral.Subscription.Helpers;

/// <summary>
/// Utility class for printing styled messages to the console.
/// </summary>
public static class ConsolePrinter
{
    /// <summary>
    /// Prints a success message in green with [SUC] level.
    /// </summary>
    public static void Success(string message)
    {
        Print($"[SUC] {message}", ConsoleColor.Green);
    }

    /// <summary>
    /// Prints an error message in red with [ERR] level.
    /// </summary>
    public static void Error(string message)
    {
        Print($"[ERR] {message}", ConsoleColor.Red);
    }

    /// <summary>
    /// Prints an informational message in cyan with [INF] level.
    /// </summary>
    public static void Info(string message)
    {
        Print($"[INF] {message}", ConsoleColor.Cyan);
    }

    /// <summary>
    /// Prints a warning message in yellow with [WRN] level.
    /// </summary>
    public static void Warning(string message)
    {
        Print($"[WRN] {message}", ConsoleColor.Yellow);
    }

    /// <summary>
    /// Prints a debug message with [DBG] level and no color.
    /// </summary>
    public static void Debug(string message)
    {
        Console.WriteLine($"[DBG] {message}");
    }

    /// <summary>
    /// Prints a message in the specified console color.
    /// </summary>
    private static void Print(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintBanner()
    {
        const string project = $"Project: {Constants.General.AppName}";

        var lines = new[]
        {
            project,
            "Purpose: Provides functionality to create, retrieve, and delete subscriptions.",
            "Prerequisites:",
            "1. A .env file must be created at the root of the project and set to 'Copy always' in Visual Studio.",
            "   Learn more about RingCentral environment variables at:",
            "   https://developers.ringcentral.com/guide/basics/code-samples",
            "2. The webhook project must be running, and its URL must be set in the .env file."
        };

        var maxWidth = lines.Max(line => line.Length);
        var border = new string('=', maxWidth + 4);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(border);
        foreach (var line in lines)
        {
            Console.WriteLine($"= {line.PadRight(maxWidth)} =");
        }
        Console.WriteLine(border);
        Console.Write(Environment.NewLine);
        Console.ResetColor();
    }

    public static void PrintExtensionsTable(List<GetExtensionListInfoResponse> extensions)
    {
        const int idWidth = 12;
        const int extNumWidth = 13;

        // Dynamically calculate the name and status column widths based on the longest values
        int nameWidth = extensions.Max(ext => (ext.name?.Length ?? 0)) + 2;  // Add extra padding
        int statusWidth = extensions.Max(ext => (ext.status?.Length ?? 0)) + 2; // Add extra padding for Status column

        // Add the Status column width to the total column widths
        string topBorder = $"┌{new string('─', idWidth)}┬{new string('─', nameWidth)}┬{new string('─', extNumWidth)}┬{new string('─', statusWidth)}┐";
        string midBorder = $"├{new string('─', idWidth)}┼{new string('─', nameWidth)}┼{new string('─', extNumWidth)}┼{new string('─', statusWidth)}┤";
        string bottomBorder = $"└{new string('─', idWidth)}┴{new string('─', nameWidth)}┴{new string('─', extNumWidth)}┴{new string('─', statusWidth)}┘";

        // Print the top border
        Console.WriteLine(topBorder);

        // Print the header row with dynamic column width, including the new Status column
        string header = string.Format("│ {0,-" + (idWidth - 1) + "}│ {1,-" + (nameWidth - 1) + "}│ {2,-" + (extNumWidth - 1) + "}│ {3,-" + (statusWidth - 1) + "}│", "ID", "Name", "Ext Number", "Status");
        Console.WriteLine(header);

        // Print the middle border
        Console.WriteLine(midBorder);

        // Print each extension's data row, including the Status column
        foreach (var ext in extensions)
        {
            string id = ext.id.ToString()!.PadRight(idWidth - 1);
            string name = (ext.contact != null ? $"{ext.contact.firstName} {ext.contact.lastName}" : "").PadRight(nameWidth - 1);
            string extNumber = (ext.extensionNumber ?? "").PadRight(extNumWidth - 1);
            string status = (ext.status ?? "").PadRight(statusWidth - 1);  // New Status field

            Console.WriteLine($"│ {id}│ {name}│ {extNumber}│ {status}│");
        }

        // Print the bottom border
        Console.WriteLine(bottomBorder);
    }

}