using InvoiceBuilder.Application.Entities;

namespace InvoiceBuilder.Application.Interfaces
{
    public interface IInvoiceDataSource
    {
        Invoice GetLatestInvoice(string filePath);
    }
}