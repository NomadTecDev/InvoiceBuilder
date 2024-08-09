using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Application.Services;

internal class InvoiceProcessor(
    ILogger<InvoiceProcessor> logger, 
    IConfigurationMapper configurationMapper,
    IWordTemplateProcessor wordTemplateProcessor,
    IDocumentGenerator docomentGenerator,
    IDateTimeProvider dateTimeProvider,
    InvoiceSettings invoiceSettings) : IInvoiceProcessor {

    public Invoice Create(RawInvoiceRow rawInvoiceRow)
    {
        try
        {
            var invoice = configurationMapper.MapToEntity<Invoice>(invoiceSettings.SourceMapping, rawInvoiceRow);
            Build(invoice);
            return invoice;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the invoice.");
            throw new Exception("An error occurred while creating the invoice.", ex);
        }
    }

    public string Generate(Invoice invoice)
    {
        try
        {
            var keyValuePairs = configurationMapper.GetEntityVariables(invoiceSettings.OutputMapping, invoice);
            var wordDocumentContents = wordTemplateProcessor.Create(invoiceSettings.TemplateFile, keyValuePairs);

            return docomentGenerator.Create(wordDocumentContents, invoiceSettings.OutputPath, invoice.InvoiceNumber);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while generating the invoice.");
            throw new Exception("An error occurred while generating the invoice.", ex);
        }
    }

    private void Build(Invoice invoice)
    {
        if (invoice.InvoiceRows is null || invoice.InvoiceRows.Count == 0)
        {
            return;
        }

        invoice.Subtotal = 0;
        invoice.VatTotal = 0;

        foreach (var invoiceRow in invoice.InvoiceRows)
        {
            // skip empty rows
            if (invoiceRow.Cost is null) continue;

            // set default VAT rate if not provided
            invoiceRow.VatRate ??= invoiceSettings.DefaultVateRate;

            // calculate the subtotal, VAT total, and total
            invoice.Subtotal += Math.Round((decimal)invoiceRow.Cost, 2);
            invoice.VatTotal += Math.Round((decimal)invoiceRow.Cost * (decimal)invoiceRow.VatRate / 100, 2);
        }

        invoice.Total = invoice.Subtotal + invoice.VatTotal;

        // set default vat reate
        invoice.VatRate ??= invoiceSettings.DefaultVateRate;

        // set expire days with default if not set
        invoice.ExpireDays ??= invoiceSettings.DefaultExpireDays;

        // set today as invoice date if not set
        invoice.InvoiceDate ??= dateTimeProvider.Today;

        // set the expire date
        invoice.ExpireDate ??= ((DateOnly)invoice.InvoiceDate).AddDays((int)invoice.ExpireDays);

        // set the default currency
        invoice.Currency ??= invoiceSettings.DefaultCurrency;
    }
}
