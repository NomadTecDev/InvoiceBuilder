using InvoiceBuilder.Core.Interfaces;
using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Ifrastructure.Repositories;

public class ExcelRepository : IInvoiceRepository
{
    public Invoice GetLatestInvoice(string filePath)
    {
        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
        {
            return null;
        }

        // Get the header row
        var headerRow = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column]
            .ToDictionary(cell => cell.Text, cell => cell.Start.Column);

        // Get the last row
        var lastRow = worksheet.Dimension.End.Row;

        var latestInvoiceDto = new ExcelInvoiceDto
        {
            InvoiceNumber = worksheet.Cells[lastRow, headerRow["InvoiceNumber"]].Text,
            InvoiceDate = DateOnly.Parse(worksheet.Cells[lastRow, headerRow["InvoiceDate"]].Text, CultureInfo.InvariantCulture),
            ExpireDate = DateOnly.Parse(worksheet.Cells[lastRow, headerRow["ExpireDate"]].Text, CultureInfo.InvariantCulture),
            Currency = worksheet.Cells[lastRow, headerRow["Currency"]].Text,
            CompanyName = worksheet.Cells[lastRow, headerRow["CompanyName"]].Text,
            ContactName = worksheet.Cells[lastRow, headerRow["ContactName"]].Text,
            Address = worksheet.Cells[lastRow, headerRow["Address"]].Text,
            Postal = worksheet.Cells[lastRow, headerRow["Postal"]].Text,
            City = worksheet.Cells[lastRow, headerRow["City"]].Text,
            Country = worksheet.Cells[lastRow, headerRow["Country"]].Text,
            ChamberOfCommerce = worksheet.Cells[lastRow, headerRow["ChamberOfCommerce"]].Text,
            VATID = worksheet.Cells[lastRow, headerRow["VATID"]].Text,
            Row1Description = worksheet.Cells[lastRow, headerRow["Row1Description"]].Text,
            Row1VatRate = GetNullableDecimal(worksheet.Cells[lastRow, headerRow["Row1VatRate"]].Text),
            Row1Cost = decimal.Parse(worksheet.Cells[lastRow, headerRow["Row1Cost"]].Text),
            Row2Description = worksheet.Cells[lastRow, headerRow["Row2Description"]].Text,
            Row2VatRate = GetNullableDecimal(worksheet.Cells[lastRow, headerRow["Row2VatRate"]].Text),
            Row2Cost = decimal.Parse(worksheet.Cells[lastRow, headerRow["Row2Cost"]].Text),
            Row3Description = worksheet.Cells[lastRow, headerRow["Row3Description"]].Text,
            Row3VatRate = GetNullableDecimal(worksheet.Cells[lastRow, headerRow["Row3VatRate"]].Text),
            Row3Cost = decimal.Parse(worksheet.Cells[lastRow, headerRow["Row3Cost"]].Text),
            Row4Description = worksheet.Cells[lastRow, headerRow["Row4Description"]].Text,
            Row4VatRate = GetNullableDecimal(worksheet.Cells[lastRow, headerRow["Row4VatRate"]].Text),
            Row4Cost = decimal.Parse(worksheet.Cells[lastRow, headerRow["Row4Cost"]].Text),
            Row5Description = worksheet.Cells[lastRow, headerRow["Row5Description"]].Text,
            Row5VatRate = GetNullableDecimal(worksheet.Cells[lastRow, headerRow["Row5VatRate"]].Text),
            Row5Cost = decimal.Parse(worksheet.Cells[lastRow, headerRow["Row5Cost"]].Text),
            Row6Description = worksheet.Cells[lastRow, headerRow["Row6Description"]].Text,
            Row6VatRate = GetNullableDecimal(worksheet.Cells[lastRow, headerRow["Row6VatRate"]].Text),
            Row6Cost = decimal.Parse(worksheet.Cells[lastRow, headerRow["Row6Cost"]].Text)
        };

        var company = new Company
        {
            Name = latestInvoiceDto.CompanyName,
            ContactName = latestInvoiceDto.ContactName,
            Address = latestInvoiceDto.Address,
            Postal = latestInvoiceDto.Postal,
            City = latestInvoiceDto.City,
            Country = latestInvoiceDto.Country,
            ChamberOfCommerce = latestInvoiceDto.ChamberOfCommerce,
            VATID = latestInvoiceDto.VATID
        };

        var invoiceRows = new List<InvoiceRow>
        {
            new InvoiceRow
            {
                Description = latestInvoiceDto.Row1Description,
                VatRate = latestInvoiceDto.Row1VatRate,
                Cost = latestInvoiceDto.Row1Cost
            },
            new InvoiceRow
            {
                Description = latestInvoiceDto.Row2Description,
                VatRate = latestInvoiceDto.Row2VatRate,
                Cost = latestInvoiceDto.Row2Cost
            },
            new InvoiceRow
            {
                Description = latestInvoiceDto.Row3Description,
                VatRate = latestInvoiceDto.Row3VatRate,
                Cost = latestInvoiceDto.Row3Cost
            },
            new InvoiceRow
            {
                Description = latestInvoiceDto.Row4Description,
                VatRate = latestInvoiceDto.Row4VatRate,
                Cost = latestInvoiceDto.Row4Cost
            },
            new InvoiceRow
            {
                Description = latestInvoiceDto.Row5Description,
                VatRate = latestInvoiceDto.Row5VatRate,
                Cost = latestInvoiceDto.Row5Cost
            },
            new InvoiceRow
            {
                Description = latestInvoiceDto.Row6Description,
                VatRate = latestInvoiceDto.Row6VatRate,
                Cost = latestInvoiceDto.Row6Cost
            }
        };

        var invoice = new Invoice
        {
            Number = latestInvoiceDto.InvoiceNumber,
            Date = latestInvoiceDto.InvoiceDate,
            ExpireDate = latestInvoiceDto.ExpireDate,
            Currency = latestInvoiceDto.Currency,
            Company = company,
            InvoiceRows = invoiceRows
        };

        return invoice;
    }

    private decimal? GetNullableDecimal(string text)
    {
        return decimal.TryParse(text, out var value) ? value : (decimal?)null;
    }
}
