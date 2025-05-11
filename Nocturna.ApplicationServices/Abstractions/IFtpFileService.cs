using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

/// <summary>
/// Provides FTP-based file operations.
/// </summary>
public interface IFtpFileService
{
    /// <summary>
    /// Checks if the target file exists on the FTP server.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that resolves to <c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
    Task<bool> FileExistsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes data to a file on the FTP server, replacing any existing content.
    /// </summary>
    /// <param name="fileExists">Specify file exists or not.</param>
    /// <param name="data">The content to write to the file.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteToFileAsync(bool fileExists, TranscriptionEntry data, CancellationToken cancellationToken = default);
}
