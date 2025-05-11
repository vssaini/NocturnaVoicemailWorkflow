using System.ComponentModel.DataAnnotations;

namespace Nocturna.Domain.Config;

/// <summary>
/// Configuration settings for connecting to an FTP server.
/// </summary>
public class FtpSettings
{
    /// <summary>
    /// Gets the hostname or IP address of the FTP server.
    /// </summary>
    [Required]
    public string Hostname { get; init; }

    /// <summary>
    /// Gets the username for FTP server authentication.
    /// </summary>
    [Required]
    public string Username { get; init; }

    /// <summary>
    /// Gets the password for FTP server authentication.
    /// </summary>
    [Required]
    public string Password { get; init; }

    /// <summary>
    /// Gets the root directory path on the FTP server for file operations.
    /// </summary>
    [Required]
    public string RootDirectory { get; init; }

    /// <summary>
    /// Gets the name of the Excel file for logging voicemail transcription.
    /// </summary>
    [Required]
    public string ExcelFileName { get; init; }

    /// <summary>
    /// Gets a value indicating whether to log FTP activity to the console. Defaults to false.
    /// </summary>
    public bool LogToConsole { get; init; } = false;

    /// <summary>
    /// Gets the timeout for reading FTP server responses, in seconds. Defaults to 10.
    /// </summary>
    public int ReadTimeoutInSeconds { get; init; } = 10;
}