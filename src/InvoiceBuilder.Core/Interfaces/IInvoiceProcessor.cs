using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IInvoiceProcessor
{
    Invoice Create(RawInvoiceRow rawInvoiceRow);
    string Generate(Invoice invoice);
}
