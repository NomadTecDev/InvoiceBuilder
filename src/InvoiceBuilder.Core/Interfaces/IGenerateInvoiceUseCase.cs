using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

public interface IGenerateInvoiceUseCase
{
    void Execute(Invoice invoice, string wordTemplateFile, string outputPdfPath);
}
