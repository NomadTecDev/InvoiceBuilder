using InvoiceBuilder.Core.Interfaces;

namespace InvoiceBuilder.Application.Services;

internal class DocumentGeneratorWord() : IDocumentGenerator
{
    public string Create(byte[] wordDocumentContents, string outputPath, string documentName)
    {
        string wordPath = Path.Combine(outputPath, $"{documentName}.docx");
        File.WriteAllBytes(wordPath, wordDocumentContents);

        return wordPath;
    }
}
