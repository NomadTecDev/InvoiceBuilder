using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

public interface IInvoiceRepository
{
    RawInvoiceRow GetRawData(string sourceFile);
}
