using DocumentFormat.OpenXml.Packaging;
using InvoiceBuilder.Core.Enums;

namespace InvoiceBuilder.Core.Interfaces;
internal interface IFileGenerator
{
    string Create(byte[] wordDocumentContents, string filename);
}
