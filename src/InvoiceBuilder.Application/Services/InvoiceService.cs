﻿using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Application.Services;

internal class InvoiceService(
    ILogger<InvoiceService> logger,
    IInvoiceRepository invoiceRepository,
    IInvoiceProcessor invoiceProcessor,
    InvoiceSettings invoiceSettings) : IInvoiceService { 

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

    public string GenerateInvoice(Invoice invoice)
    {
        try
        {
            return invoiceProcessor.Generate(invoice);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while generating the invoice.");
            throw new Exception("An error occurred while generating the invoice.", ex);
        }
    }
}
