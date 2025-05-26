using FluentFTP;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Config;
using Nocturna.Domain.Models;

namespace Nocturna.Infrastructure.Services;

public class FtpFileService(
    IOptions<FtpSettings> options,
    ILogger<FtpFileService> logger,
    IFtpClientService ftpClientService,
    IExcelFileService excelFileService)
    : IFtpFileService
{
    private readonly FtpSettings _ftpSettings = options.Value;

    public async Task<bool> FileExistsAsync(string payloadUuid, CancellationToken cancellationToken = default)
    {
        var excelFilePath = GetRemoteExcelFilePath();
        return await ftpClientService.FileExistsAsync(payloadUuid, excelFilePath, cancellationToken);
    }

    public async Task WriteToFileAsync(bool fileExists, TranscriptionEntry entry, CancellationToken cancellationToken = default)
    {
        var excelFilePath = GetRemoteExcelFilePath();
        logger.LogDebug("Payload {PayloadUuid} - Starting to write transcription to Excel file at path {ExcelFilePath}", entry.Uuid, excelFilePath);

        var stream = fileExists
            ? await ftpClientService.DownloadFileStreamAsync(entry.Uuid, excelFilePath, cancellationToken)
            : excelFileService.GenerateExcelStream(true, entry);

        if (fileExists)
            stream = excelFileService.AddNewRowToExcelFile(stream, entry);

        var status = await ftpClientService.UploadFileStreamAsync(entry.Uuid, stream, excelFilePath, cancellationToken);
        LogUploadStatus(status, entry.Uuid);
    }

    private string GetRemoteExcelFilePath()
    {
        return $"{_ftpSettings.RootDirectory.TrimEnd('/')}/{_ftpSettings.ExcelFileName.TrimStart('/')}";
    }

    private void LogUploadStatus(FtpStatus status, string payloadUuid)
    {
        if (status == FtpStatus.Success)
            logger.LogInformation("Payload {PayloadUuid} - ✅ Successfully uploaded transcription to FTP file", payloadUuid);
        else
            logger.LogError("Payload {PayloadUuid} - Failed to upload transcription to FTP file. Status: {FtpStatus}", payloadUuid, status);
    }
}