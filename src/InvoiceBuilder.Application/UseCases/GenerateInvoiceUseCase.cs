using InvoiceBuilder.Application.UseCases.Interfaces;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Application.UseCases;

internal class GenerateInvoiceUseCase(
    ILogger<GenerateInvoiceUseCase> logger,
    IInvoiceProcessor invoiceProcessor) : IGenerateInvoiceUseCase
{
    public string Execute(Invoice invoice)
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
