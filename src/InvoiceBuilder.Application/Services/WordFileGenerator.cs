using DocumentFormat.OpenXml.Packaging;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Enums;
using InvoiceBuilder.Core.Interfaces;

namespace InvoiceBuilder.Application.Services;

internal class WordFileGenerator() : IFileGenerator
{
    public string Create(byte[] wordDocumentContents, string documentName)
    {
        string wordPath = $"{documentName}.docx";
        File.CreateText(wordPath).Write(wordDocumentContents);
        return wordPath;
    }
}
