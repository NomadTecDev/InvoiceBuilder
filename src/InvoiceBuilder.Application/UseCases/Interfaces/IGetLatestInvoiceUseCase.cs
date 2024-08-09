using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Application.UseCases.Interfaces;

internal interface IGetLatestInvoiceUseCase
{
    Invoice Execute();
}
