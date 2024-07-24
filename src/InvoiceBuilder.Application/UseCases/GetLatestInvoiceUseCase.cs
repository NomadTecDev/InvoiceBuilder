using InvoiceBuilder.Application.Entities;
using InvoiceBuilder.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Application.UseCases
{
    public class GetLatestInvoiceUseCase : IGetLatestInvoiceUseCase
    {
        private readonly IInvoiceDataSource _invoiceDataSource;
        private readonly ILogger<GetLatestInvoiceUseCase> _logger;

        public GetLatestInvoiceUseCase(IInvoiceDataSource invoiceDataSource, ILogger<GetLatestInvoiceUseCase> logger)
        {
            _invoiceDataSource = invoiceDataSource;
            _logger = logger;
        }

        public Invoice Execute(string filePath)
        {
            try
            {
                var invoice = _invoiceDataSource.GetLatestInvoice(filePath);
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
}
