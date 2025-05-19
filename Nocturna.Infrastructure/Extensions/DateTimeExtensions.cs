using System.Globalization;
using System.Runtime.InteropServices;

namespace Nocturna.Infrastructure.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a given UTC <see cref="DateTime"/> to the Pacific Time Zone (PST/PDT) and formats it 
    /// as "MM/dd/yyyy HH:mm" in a 24-hour format. The conversion automatically handles Daylight Saving Time (DST).
    /// </summary>
    /// <param name="dateTime">The UTC <see cref="DateTime"/> to be converted to Pacific Time.</param>
    /// <returns>A string representing the time in the Pacific Time Zone (either PST or PDT) in the format "MM/dd/yyyy HH:mm".</returns>
    public static string ToPacificTime(this DateTime dateTime)
    {
        var pacificTimeZone = GetPacificTimeZone();

        // Convert UTC time to Pacific Standard Time, accounting for DST automatically
        var pacificTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, pacificTimeZone);

        // Return the formatted time as MM/dd/yyyy HH:mm (24-hour format)
        return pacificTime.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
    }

    private static TimeZoneInfo GetPacificTimeZone()
    {
        // Use IANA ID for Linux, Windows ID for Windows
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")
            : TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
    }
}