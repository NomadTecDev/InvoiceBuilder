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
            GenerateInvoice(invoice);
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
            var rawJsonString = @"
                {
                'InvoiceNumber': '${InvoiceNumber}',
                'InvoiceDate': '${InvoiceDate}',
                'InvoiceRows': [
                    {
                        'Description': '${IR1}',
                        'Cost': ${IRP1}

                    },
                    {
                        'Description': '${IR2}',
                        'Cost': ${IRP2}
                    },
                    {
                        'Description': '${IR3}',
                        'Cost': ${IRP3}
                    },
                    {
                        'Description': '${IR4}',
                        'Cost': ${IRP4}
                    },
                    {
                        'Description': '${IR5}',
                        'Cost': ${IRP5}
                    },
                    {
                        'Description': '${IR6}',
                        'Cost': ${IRP6}
                    }
                ],
                'Company': {
                    'Name': '${Client}',
                    'ContactName': '${Contact}',
                    'Address': '${Address}',
                    'Postal': '${Postal}',
                    'City': '${City}',
                    'Country': '${Country}',
                    'ChamberOfCommerce': '${Kvk}',
                    'VATID': '${Postal}'
                    }
                 }";

            rawJsonString = rawJsonString.Replace("'", "\"");

            foreach (var key in rawInvoiceRow)
            {
                // first replace strings
                rawJsonString = Regex.Replace(rawJsonString, $"\"\\${{{key.Key}}}\"", string.IsNullOrWhiteSpace(key.Value) ? "null" : "\"" + key.Value + "\"");

                // then replace numbers
                rawJsonString = Regex.Replace(rawJsonString, $"\\${{{key.Key}}}", string.IsNullOrWhiteSpace(key.Value) ? "null" : key.Value);
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new DateOnlyJsonConverter("dd/MM/yyyy")
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

    private void GenerateInvoice(Invoice invoice)
    {
        if(invoice.InvoiceRows is null || invoice.InvoiceRows.Count == 0)
        {
            return;
        }

        invoice.Subtal = 0;
        invoice.VatTotal = 0;

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

        // set the expire date
        invoice.ExpireDate ??= invoice.InvoiceDate.AddDays(invoiceSettings.DefaultExpireDays);

        // set the default currency
        invoice.Currency ??= invoiceSettings.DefaultCurrency;
    }
    public class DateOnlyJsonConverter(string dateFormat) : JsonConverter<DateOnly>
    {
        private readonly string _dateFormat = dateFormat;

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateOnly.TryParseExact(reader.GetString(), _dateFormat, null, System.Globalization.DateTimeStyles.None, out DateOnly date))
            {
                return date;
            }

            throw new JsonException($"Unable to parse date. Expected format: {_dateFormat}");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_dateFormat));
        }
    }
}
