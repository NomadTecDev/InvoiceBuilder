using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

public interface IInvoiceMapper
{
    Invoice MapSource(RawInvoiceRow rawInvoiceRow);
}
