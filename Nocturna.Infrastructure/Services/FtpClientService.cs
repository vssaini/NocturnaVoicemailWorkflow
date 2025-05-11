using FluentFTP;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Config;
using Nocturna.Infrastructure.Policies;

namespace Nocturna.Infrastructure.Services;

public class FtpClientService(IOptions<FtpSettings> options, ILogger<FtpClientService> logger) : IFtpClientService
{
    private readonly FtpSettings _ftp = options.Value;

    public async Task<bool> FileExistsAsync(string remoteFilePath, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var client = await CreateFtpClientAsync(cancellationToken);
            return await client.FileExists(remoteFilePath, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking file existence: {FilePath}", remoteFilePath);
            return false;
        }
    }

    private async Task<AsyncFtpClient> CreateFtpClientAsync(CancellationToken cancellationToken)
    {
        var client = new AsyncFtpClient(_ftp.Hostname, _ftp.Username, _ftp.Password)
        {
            Config =
            {
                LogToConsole = _ftp.LogToConsole,
                ReadTimeout = _ftp.ReadTimeoutInSeconds * 1000,
                EncryptionMode = FtpEncryptionMode.Explicit
            }
        };
        await client.Connect(cancellationToken);
        return client;
    }

    public async Task<MemoryStream> DownloadFileStreamAsync(string remoteFilePath, CancellationToken cancellationToken)
    {
        logger.LogInformation("Downloading existing file stream from FTP server {FilePath}", remoteFilePath);

        var stream = new MemoryStream();
        await using (var client = await CreateFtpClientAsync(cancellationToken))
        {
            await client.Connect(cancellationToken);
            await client.DownloadStream(stream, remoteFilePath, token: cancellationToken);
        }
        stream.Position = 0;
        return stream;
    }

    public async Task<FtpStatus> UploadFileStreamAsync(MemoryStream stream, string remoteFilePath,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Uploading file stream to FTP server {FilePath}", remoteFilePath);

        var filePolicy = FilePolicy.CreateFtpRetryPolicy(logger);

        await using var client = await CreateFtpClientAsync(cancellationToken);
        var status = await filePolicy.ExecuteAsync(ct => client.UploadStream(
            stream,
            remotePath: remoteFilePath,
            existsMode: FtpRemoteExists.Overwrite,
            createRemoteDir: true,
            token: ct
        ), cancellationToken);

        return status;
    }
}