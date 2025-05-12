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
        var lines = new[]
        {
            "Project: RingCentral.Subscription",
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
}