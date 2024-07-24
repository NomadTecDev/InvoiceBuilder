using InvoiceBuilder.Application.Entities;
using InvoiceBuilder.Application.Interfaces;
using InvoiceBuilder.Application.UseCases;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IGetLatestInvoiceUseCase _getLatestInvoiceUseCase;
        private readonly IGenerateInvoiceUseCase _generateInvoiceUseCase;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(
            IGetLatestInvoiceUseCase getLatestInvoiceUseCase,
            IGenerateInvoiceUseCase generateInvoiceUseCase,
            ILogger<InvoiceService> logger)
        {
            _getLatestInvoiceUseCase = getLatestInvoiceUseCase;
            _generateInvoiceUseCase = generateInvoiceUseCase;
            _logger = logger;
        }

        public Invoice GetLatestInvoice(string filePath)
        {
            try
            {
                return _getLatestInvoiceUseCase.Execute(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the latest invoice.");
                throw;
            }
        }

        public void GenerateInvoiceDocument(Invoice invoice, string wordTemplatePath, string outputPdfPath)
        {
            try
            {
                _generateInvoiceUseCase.Execute(invoice, wordTemplatePath, outputPdfPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the invoice document.");
                throw;
            }
        }
    }
}
