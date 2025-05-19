using FluentFTP;

namespace Nocturna.Application.Abstractions;

public interface IFtpClientService
{
    Task<bool> FileExistsAsync(string payloadUuid, string remoteFilePath, CancellationToken cancellationToken = default);
    Task<MemoryStream> DownloadFileStreamAsync(string payloadUuid, string remoteFilePath, CancellationToken cancellationToken = default);
    Task<FtpStatus> UploadFileStreamAsync(string payloadUuid, MemoryStream stream, string remoteFilePath, CancellationToken cancellationToken = default);
}