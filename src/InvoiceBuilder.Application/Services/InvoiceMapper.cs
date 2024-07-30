using DocumentFormat.OpenXml.Wordprocessing;
using InvoiceBuilder.Application.Converters;
using InvoiceBuilder.Application.Extensions;
using InvoiceBuilder.Configuration.Extensions;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace InvoiceBuilder.Application.Services;

internal class InvoiceMapper(
    ILogger<InvoiceService> logger, 
    InvoiceSettings invoiceSettings) : IInvoiceMapper { 

    public Invoice MapSource(RawInvoiceRow rawInvoiceRow)
    {
        var sourceMapJson = invoiceSettings.SourceMapping.GetJsonInvoice(rawInvoiceRow);

        try
        {
            var options = new JsonSerializerOptions
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

            var invoice = JsonSerializer.Deserialize<Invoice>(sourceMapJson, options)
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
