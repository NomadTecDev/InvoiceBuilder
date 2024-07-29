using Microsoft.Extensions.DependencyInjection;

namespace InvoiceBuilder.Core.Interfaces;

public interface IInvoiceRepositoryFactory
{
    IInvoiceRepository GetInvoiceRepository();
}
