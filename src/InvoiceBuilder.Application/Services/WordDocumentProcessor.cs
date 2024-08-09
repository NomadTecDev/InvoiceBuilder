using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using InvoiceBuilder.Core.Interfaces;

internal class WordDocumentProcessor() : IWordDocumentProcessor
{
    public byte[] Create(string documentTemplate, Dictionary<string, string> documentVariables)
    {
        using var memoryStream = new MemoryStream();

        using var fileStream = new FileStream(documentTemplate, FileMode.Open, FileAccess.Read);

        fileStream.CopyTo(memoryStream);

        // Open the document from the memory stream
        using (var wordDoc = WordprocessingDocument.Open(memoryStream, true))
        {
            foreach (var keyValuePair in documentVariables)
            {
                ReplacePlaceholder(wordDoc, keyValuePair.Key, keyValuePair.Value);
            }

            wordDoc.MainDocumentPart?.Document.Save();
        }

        return memoryStream.ToArray();
    }

    private static void ReplacePlaceholder(WordprocessingDocument wordDoc, string placeholder, string value)
    {
        var body = wordDoc.MainDocumentPart?.Document.Body ?? throw new NullReferenceException(nameof(wordDoc.MainDocumentPart));

        foreach (var text in body.Descendants<Text>())
        {
            if (text.Text.Contains(placeholder))
            {
                text.Text = text.Text.Replace(placeholder, value);
            }
        }
    }
}
