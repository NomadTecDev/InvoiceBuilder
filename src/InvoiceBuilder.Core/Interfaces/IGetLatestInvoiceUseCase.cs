using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

public interface IGetLatestInvoiceUseCase
{
    Invoice Execute(string filePath);
}
