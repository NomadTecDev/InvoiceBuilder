using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using InvoiceBuilder.Core.Interfaces;
using System.Reflection.Metadata;

internal class WordTemplateProcessor() : IWordTemplateProcessor
{
    public byte[] Create(string documentTemplate, Dictionary<string, string> documentVariables)
    {
        using var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(documentTemplate, FileMode.Open, FileAccess.Read))
        {
            fileStream.CopyTo(memoryStream);
        }

        using (var wordDoc = WordprocessingDocument.Open(memoryStream, true))
        {
            var documentPart = wordDoc.MainDocumentPart;

            foreach (var variable in documentVariables)
            {
                ReplacePlaceholders(wordDoc, $"#{variable.Key}#", variable.Value);
            }

            wordDoc.MainDocumentPart?.Document.Save();
        }

        return memoryStream.ToArray();
    }

    static void ReplacePlaceholders(WordprocessingDocument wordDoc, string placeholder, string replacement)
    {
        var body = wordDoc.MainDocumentPart.Document.Body;

        if (string.IsNullOrWhiteSpace(replacement))
        {
            replacement = " ";
        }

        foreach (var textElement in body.Descendants<Text>())
        {
            if (textElement.Text.Contains(placeholder))
            {
                textElement.Text = textElement.Text.Replace(placeholder, replacement);
            }
        }
    }
}
