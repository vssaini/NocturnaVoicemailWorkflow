using FluentFTP;

namespace Nocturna.Application.Abstractions;

public interface IFtpClientService
{
    Task<bool> FileExistsAsync(string remoteFilePath, CancellationToken cancellationToken = default);
    Task<MemoryStream> DownloadFileStreamAsync(string remoteFilePath, CancellationToken cancellationToken);
    Task<FtpStatus> UploadFileStreamAsync(MemoryStream stream, string remoteFilePath, CancellationToken cancellationToken);
}