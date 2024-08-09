using InvoiceBuilder.Application.UseCases.Interfaces;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Application.UseCases;

internal class GetLatestInvoiceUseCase(
    ILogger<GetLatestInvoiceUseCase> logger,
    IInvoiceRepository invoiceRepository,
    IInvoiceProcessor invoiceProcessor,
    InvoiceSettings invoiceSettings) : IGetLatestInvoiceUseCase
{
    public Invoice Execute()
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
