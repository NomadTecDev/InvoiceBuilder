using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

public interface IInvoiceBuilder
{
    Invoice GetLatestInvoice(string filePath);
    void GenerateInvoiceDocument(Invoice invoice, string wordTemplatePath, string outputPdfPath);
}
