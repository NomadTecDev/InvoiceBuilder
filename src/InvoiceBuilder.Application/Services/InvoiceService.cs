using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace InvoiceBuilder.Application.Services;

public class InvoiceService(
    ILogger<InvoiceService> logger,
    IInvoiceRepository invoiceRepository,
    IInvoiceProcessor invoiceProcessor,
    InvoiceSettings invoiceSettings) : IInvoiceService
{

    public Invoice GetLatestInvoice()
    {
        try
        {
            var rawInvoiceRow = invoiceRepository.GetRawData(invoiceSettings.SourceFile);
            return invoiceProcessor.Create(rawInvoiceRow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting the latest invoice.");
            throw new Exception("An error occurred while getting the latest invoice.", ex);
        }
    }


}
