using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

public interface IInvoiceProcessor
{
    Invoice Create(RawInvoiceRow rawInvoiceRow);
}
