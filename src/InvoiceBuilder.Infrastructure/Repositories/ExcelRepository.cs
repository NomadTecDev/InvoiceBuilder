using InvoiceBuilder.Core.Interfaces;
using InvoiceBuilder.Core.Entities;
using OfficeOpenXml;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Ifrastructure.Repositories;

public class ExcelInvoiceRepository(ILogger<ExcelInvoiceRepository> logger) : IInvoiceRepository
{
    private readonly ILogger<ExcelInvoiceRepository> logger = logger;

    public RawInvoiceRow GetRawData(string sourceFile)
    {
        try
        {
            using var package = new ExcelPackage(new FileInfo(sourceFile));
            var worksheet = package.Workbook.Worksheets[1];
            var lastRow = worksheet.Dimension.End.Row;

            var rawInvoiceRow = new RawInvoiceRow();

            for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
            {
                var headerCell = worksheet.Cells[1, i].Value?.ToString();
                var dataCell = worksheet.Cells[lastRow, i].Value?.ToString();

                if (headerCell is not null)
                {
                    rawInvoiceRow.Add(headerCell, dataCell);
                }
            }

            return rawInvoiceRow;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while reading the Excel file {SourceFile}.", sourceFile);
            throw new Exception($"An error occurred while reading the Excel file {sourceFile}.", ex);
        }
    }
}
