using InvoiceBuilder.Application.Entities;

namespace InvoiceBuilder.Application.UseCases
{
    public interface IGetLatestInvoiceUseCase
    {
        Invoice Execute(string filePath);
    }
}
