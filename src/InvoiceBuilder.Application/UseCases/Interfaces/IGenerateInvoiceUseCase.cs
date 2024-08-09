using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Application.UseCases.Interfaces;

internal interface IGenerateInvoiceUseCase
{
    string Execute(Invoice invoice);
}
