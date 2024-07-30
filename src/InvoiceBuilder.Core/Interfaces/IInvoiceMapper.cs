using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IInvoiceMapper
{
    Invoice MapSource(RawInvoiceRow rawInvoiceRow);
}
