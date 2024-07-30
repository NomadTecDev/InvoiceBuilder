using InvoiceBuilder.Application.Converters;
using InvoiceBuilder.Application.Extensions;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InvoiceBuilder.Application.Services;

internal class InvoiceMapper(
    ILogger<InvoiceService> logger, 
    InvoiceSettings invoiceSettings) : IInvoiceMapper { 

    private readonly JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReadCommentHandling = JsonCommentHandling.Skip, 
                AllowTrailingCommas = true, 
                Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                        new StringToDecimalConverter(),
                        new DecimalToStringConverter(),
                        new DateOnlyJsonConverter(invoiceSettings.DefaultDateFormat)
                }
            };

public Invoice MapSource(RawInvoiceRow rawInvoiceRow)
    {
        var sourceMapJson = invoiceSettings.SourceMapping.GetJsonInvoice(rawInvoiceRow);

        try
        {
            var invoice = JsonSerializer.Deserialize<Invoice>(sourceMapJson, jsonSerializerOptions)
                ?? throw new Exception("Deserialization resulted in null Invoice object");

            return invoice;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to Map the invoice based on it's mapping settings");
            throw new Exception("Unable to Map the invoice based on it's mapping settings", ex);
        }
    }
}
