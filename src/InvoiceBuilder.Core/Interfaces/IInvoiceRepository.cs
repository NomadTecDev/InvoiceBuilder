using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IInvoiceRepository
{
    RawInvoiceRow GetRawData(string sourceFile);
}
