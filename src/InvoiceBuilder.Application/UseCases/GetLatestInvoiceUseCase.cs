using InvoiceBuilder.Application.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Application.UseCases;

public class GetLatestInvoiceUseCase(IInvoiceRepository invoiceRepository, ILogger<GetLatestInvoiceUseCase> logger) : IGetLatestInvoiceUseCase
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly ILogger<GetLatestInvoiceUseCase> _logger = logger;

    public Invoice Execute(string filePath)
    {
        try
        {
            var invoice = _invoiceRepository.GetLatestInvoice(filePath);
            if (invoice == null)
            {
                _logger.LogWarning("No invoice found in the specified file.");
                return null;
            }

            _logger.LogInformation("Successfully retrieved the latest invoice.");
            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the latest invoice.");
            throw;
        }
    }
}
