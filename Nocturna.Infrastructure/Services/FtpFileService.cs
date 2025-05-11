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

    public async Task<bool> FileExistsAsync(CancellationToken cancellationToken = default)
    {
        var excelFilePath = GetRemoteExcelFilePath();
        return await ftpClientService.FileExistsAsync(excelFilePath, cancellationToken);
    }

    public async Task WriteToFileAsync(bool fileExists, TranscriptionEntry entry, CancellationToken cancellationToken = default)
    {
        var excelFilePath = GetRemoteExcelFilePath();
        logger.LogInformation("Beginning to save transcription payload {PayloadUuid} to Excel file at path: {ExcelFilePath}", entry.Uuid, excelFilePath);

        var stream = fileExists
            ? await ftpClientService.DownloadFileStreamAsync(excelFilePath, cancellationToken)
            : excelFileService.GenerateExcelStream(true, entry);

        if (fileExists)
            stream = excelFileService.AddNewRowToExcelFile(stream, entry);

        var status = await ftpClientService.UploadFileStreamAsync(stream, excelFilePath, cancellationToken);
        LogUploadStatus(status, entry.Uuid);
    }

    private string GetRemoteExcelFilePath()
    {
        return $"{_ftpSettings.RootDirectory.TrimEnd('/')}/{_ftpSettings.ExcelFileName.TrimStart('/')}";
    }

    private void LogUploadStatus(FtpStatus status, string ringCentralUuid)
    {
        if (status == FtpStatus.Success)
            logger.LogInformation("Successfully saved transcription payload {PayloadUuid} to Excel file", ringCentralUuid);
        else
            logger.LogError("Failed to save transcription payload {PayloadUuid} to Excel file. FTP status: {FtpStatus}", ringCentralUuid, status);
    }
}