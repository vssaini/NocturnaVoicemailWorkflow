using ClosedXML.Excel;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Infrastructure.Extensions;

namespace Nocturna.Infrastructure.Services;

public class ExcelFileService : IExcelFileService
{
    public MemoryStream GenerateExcelStream(bool includeHeaders, TranscriptionEntry entry)
    {
        var stream = new MemoryStream();
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Transcriptions");

        var row = 1;
        if (includeHeaders)
        {
            AddExcelHeaders(worksheet);
            row++;
        }

        AddExcelRow(worksheet, row, entry);
        worksheet.ColumnsUsed().AdjustToContents();

        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    private static void AddExcelHeaders(IXLWorksheet sheet)
    {
        sheet.Cell(1, 1).Value = "UUID";
        sheet.Cell(1, 2).Value = "Created At";
        sheet.Cell(1, 3).Value = "From";
        sheet.Cell(1, 4).Value = "To";
        sheet.Cell(1, 5).Value = "Transcription";
    }

    private static void AddExcelRow(IXLWorksheet sheet, int row, TranscriptionEntry entry)
    {
        sheet.Cell(row, 1).Value = entry.Uuid;
        sheet.Cell(row, 2).Value = entry.CreationTime.ToPacificTime();
        sheet.Cell(row, 3).Value = entry.FromPhoneNumber;
        sheet.Cell(row, 4).Value = entry.ToPhoneNumber;
        sheet.Cell(row, 5).Value = entry.Transcription.Replace('\u202F', ' ');
    }

    public MemoryStream AddNewRowToExcelFile(MemoryStream existingStream, TranscriptionEntry entry)
    {
        using var workbook = new XLWorkbook(existingStream);
        var worksheet = workbook.Worksheets.Worksheet(1);

        var newRow = worksheet.RowsUsed().Count() + 1;
        AddExcelRow(worksheet, newRow, entry);

        worksheet.ColumnsUsed().AdjustToContents();

        return SaveExcelToStream(workbook);
    }

    private static MemoryStream SaveExcelToStream(XLWorkbook workbook)
    {
        var newStream = new MemoryStream();
        workbook.SaveAs(newStream);
        newStream.Position = 0;
        return newStream;
    }
}