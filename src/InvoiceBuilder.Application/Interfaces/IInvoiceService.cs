using InvoiceBuilder.Application.Entities;

namespace InvoiceBuilder.Application.Interfaces
{
    public interface IInvoiceService
    {
        Invoice GetLatestInvoice(string filePath);
        void GenerateInvoiceDocument(Invoice invoice, string wordTemplatePath, string outputPdfPath);
    }
}
