using InvoiceBuilder.Core.Interfaces;
using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Infrastructure.Repositories;

public class CsvInvoiceRepository : IInvoiceRepository
{
    public Invoice GetLatestInvoice(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<CsvInvoiceDto>().ToList();
        var latestInvoiceDto = records.LastOrDefault();

        if (latestInvoiceDto == null)
        {
            return null;
        }

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
}

public class CsvInvoiceDtoMap : ClassMap<CsvInvoiceDto>
{
    public CsvInvoiceDtoMap()
    {
        Map(m => m.InvoiceNumber);
        Map(m => m.InvoiceDate).TypeConverterOption.Format("yyyy-MM-dd");
        Map(m => m.ExpireDate).TypeConverterOption.Format("yyyy-MM-dd");
        Map(m => m.Currency);
        Map(m => m.CompanyName);
        Map(m => m.ContactName);
        Map(m => m.Address);
        Map(m => m.Postal);
        Map(m => m.City);
        Map(m => m.Country);
        Map(m => m.ChamberOfCommerce);
        Map(m => m.VATID);
        Map(m => m.Row1Description);
        Map(m => m.Row1VatRate).TypeConverter<NullableDecimalConverter>();
        Map(m => m.Row1Cost);
        Map(m => m.Row2Description);
        Map(m => m.Row2VatRate).TypeConverter<NullableDecimalConverter>();
        Map(m => m.Row2Cost);
        Map(m => m.Row3Description);
        Map(m => m.Row3VatRate).TypeConverter<NullableDecimalConverter>();
        Map(m => m.Row3Cost);
        Map(m => m.Row4Description);
        Map(m => m.Row4VatRate).TypeConverter<NullableDecimalConverter>();
        Map(m => m.Row4Cost);
        Map(m => m.Row5Description);
        Map(m => m.Row5VatRate).TypeConverter<NullableDecimalConverter>();
        Map(m => m.Row5Cost);
        Map(m => m.Row6Description);
        Map(m => m.Row6VatRate).TypeConverter<NullableDecimalConverter>();
        Map(m => m.Row6Cost);
    }
}

public class NullableDecimalConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        return decimal.TryParse(text, out var value) ? value : (decimal?)null;
    }
}
