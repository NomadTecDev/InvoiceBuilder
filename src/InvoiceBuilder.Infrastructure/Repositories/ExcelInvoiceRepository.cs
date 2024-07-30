using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Ifrastructure.Repositories;

internal class ExcelInvoiceRepository(ILogger<ExcelInvoiceRepository> logger) : IInvoiceRepository
{
    public RawInvoiceRow GetRawData(string sourceFile)
    {
        try
        {
            using SpreadsheetDocument document = SpreadsheetDocument.Open(sourceFile, false);

            WorkbookPart workbookPart = document.WorkbookPart;
            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            int lastRowIndex = sheetData.Elements<Row>().Count();
            Row headerRow = sheetData.Elements<Row>().First();
            RawInvoiceRow rawInvoiceRow = new RawInvoiceRow();

            foreach (Cell cell in headerRow.Elements<Cell>())
            {
                string headerCellValue = GetCellValue(cell, workbookPart);
                Cell dataCell = GetCell(sheetData, lastRowIndex, cell.CellReference);
                string dataCellValue = GetCellValue(dataCell, workbookPart);

                if (!string.IsNullOrEmpty(headerCellValue))
                {
                    rawInvoiceRow.Add(headerCellValue, dataCellValue);
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

    private string GetCellValue(Cell cell, WorkbookPart workbookPart)
    {
        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            SharedStringTablePart sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            if (sharedStringTablePart != null)
            {
                SharedStringItem sharedStringItem = sharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(cell.CellValue.InnerText));
                return sharedStringItem.Text.Text;
            }
        }

        return cell.CellValue?.InnerText;
    }

    private Cell GetCell(SheetData sheetData, int rowIndex, string cellReference)
    {
        Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
        if (row != null)
        {
            return row.Elements<Cell>().FirstOrDefault(c => c.CellReference.Value == cellReference);
        }

        return null;
    }
}
