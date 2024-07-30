using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IInvoiceService
{
    Invoice GetLatestInvoice();
}
