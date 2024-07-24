using InvoiceBuilder.Application.Entities;

namespace InvoiceBuilder.Application.Interfaces
{
    public interface IGetLatestInvoiceUseCase
    {
        Invoice Execute(string filePath);
    }
}
