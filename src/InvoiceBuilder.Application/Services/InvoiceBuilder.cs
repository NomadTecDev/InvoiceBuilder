using InvoiceBuilder.Application.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Application.Services
{
    public class InvoiceBuilder(
        IGetLatestInvoiceUseCase getLatestInvoiceUseCase,
        IGenerateInvoiceUseCase generateInvoiceUseCase,
        ILogger<InvoiceBuilder> logger) : IInvoiceBuilder
    {
        private readonly IGetLatestInvoiceUseCase _getLatestInvoiceUseCase = getLatestInvoiceUseCase;
        private readonly IGenerateInvoiceUseCase _generateInvoiceUseCase = generateInvoiceUseCase;
        private readonly ILogger<InvoiceBuilder> _logger = logger;

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
