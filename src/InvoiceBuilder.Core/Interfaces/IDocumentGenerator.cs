using DocumentFormat.OpenXml.Packaging;
using InvoiceBuilder.Core.Enums;

namespace InvoiceBuilder.Core.Interfaces;
internal interface IDocumentGenerator
{
    string Create(byte[] wordDocumentContents, string outputPath, string filename);
}
