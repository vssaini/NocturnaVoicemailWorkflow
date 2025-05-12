using System.Runtime.InteropServices;

namespace RingCentral.Subscription.Helpers;

/// <summary>
/// Provides helper methods for enabling ANSI escape sequence support in the Windows console.
/// </summary>
public static class ConsoleHelper
{
    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    /// <summary>
    /// Retrieves a handle to the specified standard device (standard input, standard output, or standard error).
    /// </summary>
    /// <param name="nStdHandle">The standard device handle to retrieve. Use -11 for STD_OUTPUT_HANDLE.</param>
    /// <returns>A handle to the specified device, or <see cref="IntPtr.Zero"/> if the function fails.</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    /// <summary>
    /// Retrieves the current input mode of a console's input buffer or the output mode of a console screen buffer.
    /// </summary>
    /// <param name="hConsoleHandle">A handle to the console input buffer or the console screen buffer.</param>
    /// <param name="lpMode">On output, the current mode of the specified buffer.</param>
    /// <returns><c>true</c> if the function succeeds; otherwise, <c>false</c>.</returns>
    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    /// <summary>
    /// Sets the input mode of a console's input buffer or the output mode of a console screen buffer.
    /// </summary>
    /// <param name="hConsoleHandle">A handle to the console input buffer or the console screen buffer.</param>
    /// <param name="dwMode">The input or output mode to be set.</param>
    /// <returns><c>true</c> if the function succeeds; otherwise, <c>false</c>.</returns>
    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    /// <summary>
    /// Enables virtual terminal processing on the Windows console, allowing ANSI escape sequences to control formatting such as colors.
    /// This must be called before using ANSI codes to ensure compatibility outside environments like Visual Studio.
    /// </summary>
    public static void EnableVirtualTerminalProcessing()
    {
        var handle = GetStdHandle(STD_OUTPUT_HANDLE);
        if (!GetConsoleMode(handle, out uint mode))
            return;

        SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
    }
}