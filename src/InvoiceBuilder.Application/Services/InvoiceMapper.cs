using InvoiceBuilder.Application.Converters;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace InvoiceBuilder.Application.Services;

public class InvoiceMapper(
    ILogger<InvoiceService> logger, 
    InvoiceSettings invoiceSettings) : IInvoiceMapper { 

    public Invoice MapSource(RawInvoiceRow rawInvoiceRow)
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
                        new DateOnlyJsonConverter(invoiceSettings.DefaultDateFormat)
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
}
