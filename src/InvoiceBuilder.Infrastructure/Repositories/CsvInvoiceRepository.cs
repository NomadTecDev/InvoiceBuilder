using CsvHelper;
using CsvHelper.Configuration;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Infrastructure.Repositories;

internal class CsvInvoiceRepository(ILogger<CsvInvoiceRepository> logger) : IInvoiceRepository
{
    public RawInvoiceRow GetRawData(string sourceFile)
    {
        try
        {
            using var reader = new StreamReader(sourceFile);
            using var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture));
            var record = csv.GetRecords<dynamic>().LastOrDefault();

            var rawInvoiceRow = new RawInvoiceRow();
            if (record != null)
            {
                foreach (var kvp in record)
                {
                    rawInvoiceRow.Add(kvp.Key, kvp.Value.ToString());
                }
            }
            logger.LogInformation("Successfully read the CSV file {SourceFile}.", sourceFile);
            return rawInvoiceRow;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while processing the file {SourceFile}.", sourceFile);
            throw new Exception($"An unexpected error occurred while processing the file {sourceFile}.", ex);
        }
    }
}
