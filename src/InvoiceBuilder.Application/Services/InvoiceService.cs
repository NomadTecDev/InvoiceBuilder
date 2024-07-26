using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace InvoiceBuilder.Application.Services;

public class InvoiceService(
    ILogger<InvoiceService> logger,
    InvoiceSettings invoiceSettings,
    IInvoiceRepository invoiceRepository) : IInvoiceService
{
    private readonly ILogger<InvoiceService> logger = logger;
    private readonly InvoiceSettings invoiceSettings = invoiceSettings;
    private readonly IInvoiceRepository invoiceRepository = invoiceRepository;

    public Invoice GetLatestInvoice()
    {
        try
        {
            var rawInvoiceRow = invoiceRepository.GetRawData(invoiceSettings.SourceFile);
            var invoice = InvoiceMapper(rawInvoiceRow);
            InvoiceCalculator(invoice);
            return invoice;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting the latest invoice.");
            throw new Exception("An error occurred while getting the latest invoice.", ex);
        }
    }

    private Invoice InvoiceMapper(RawInvoiceRow rawInvoiceRow)
    {
        try
        {
            var rawJsonString = invoiceSettings.MappingRawString;

            foreach (var key in rawInvoiceRow)
            {
                rawJsonString = Regex.Replace(rawJsonString, $"\\${{{key.Key}}}", key.Value ?? "null");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new DateOnlyJsonConverter() // Custom converter for DateOnly
                }
            };

            return JsonSerializer.Deserialize<Invoice>(rawJsonString, options)
                ?? throw new Exception("Deserialization resulted in null Invoice object");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to Map the invoice based on it's mapping settings");
            throw new Exception("Unable to Map the invoice based on it's mapping settings", ex);
        }
    }

    private void InvoiceCalculator(Invoice invoice)
    {
        if(invoice.InvoiceRows is null || invoice.InvoiceRows.Count == 0)
        {
            return;
        }
        
        foreach (var invoiceRow in invoice.InvoiceRows)
        {
            // skip empty rows
            if (invoiceRow.Cost is null) continue;

            // set default VAT rate if not provided
            invoiceRow.VatRate ??= invoiceSettings.DefaultVateRate;

            // calculate the subtotal, VAT total, and total
            invoice.Subtal += Math.Round((decimal)invoiceRow.Cost, 2);
            invoice.VatTotal += Math.Round((decimal)invoiceRow.Cost * (decimal)invoiceRow.VatRate / 100, 2);
        }

        invoice.Total = invoice.Subtal + invoice.VatTotal;
    }

    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && reader.TryGetDateTime(out DateTime dateTime))
            {
                return DateOnly.FromDateTime(dateTime);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("MM/dd/yyyy"));
        }
    }
}
