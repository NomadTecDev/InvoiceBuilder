using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

public interface IInvoiceRepository
{
    Invoice GetLatestInvoice(string filePath);
}