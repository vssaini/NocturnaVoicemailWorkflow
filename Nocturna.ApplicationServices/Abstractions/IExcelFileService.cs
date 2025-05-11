using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

public interface IExcelFileService
{
    MemoryStream GenerateExcelStream(bool includeHeaders, TranscriptionEntry entry);
    MemoryStream AddNewRowToExcelFile(MemoryStream existingStream, TranscriptionEntry entry);
}