using DocumentFormat.OpenXml.Packaging;
using InvoiceBuilder.Core.Enums;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IWordTemplateProcessor
{
    byte[] Create(string documentTemplate, Dictionary<string, string> documentVariables);
}
