using Microsoft.Extensions.DependencyInjection;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IInvoiceRepositoryFactory
{
    IInvoiceRepository GetInvoiceRepository();
}
