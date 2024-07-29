using InvoiceBuilder.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static InvoiceBuilder.Application.Services.InvoiceService;
using InvoiceBuilder.Application.Converters;
using InvoiceBuilder.Core.Interfaces;

namespace InvoiceBuilder.Application.Services;

public class InvoiceProcessor(
    ILogger<InvoiceService> logger, 
    IInvoiceMapper invoiceMapper,
    InvoiceSettings invoiceSettings) : IInvoiceProcessor {

    public Invoice Create(RawInvoiceRow rawInvoiceRow)
    {
        try
        {
            var invoice = invoiceMapper.MapSource(rawInvoiceRow);
            Build(invoice);
            return invoice;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the invoice.");
            throw new Exception("An error occurred while creating the invoice.", ex);
        }
    }

    private void Build(Invoice invoice)
    {
        if (invoice.InvoiceRows is null || invoice.InvoiceRows.Count == 0)
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
}
