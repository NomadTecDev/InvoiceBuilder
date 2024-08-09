using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;

namespace InvoiceBuilder.Application.Services;

internal class DocumentGeneratorSyncfusionPDF(ILogger<DocumentGeneratorSyncfusionPDF> logger) : IDocumentGenerator
{
    public string Create(byte[] wordDocumentContents, string outputPath, string documentName)
    {
        ArgumentNullException.ThrowIfNull(wordDocumentContents, nameof(wordDocumentContents));
        ArgumentException.ThrowIfNullOrEmpty(outputPath, nameof(outputPath));
        ArgumentException.ThrowIfNullOrEmpty(documentName, nameof(documentName));

        string pdfPath = Path.Combine(outputPath, $"{documentName}.pdf");

        try
        {
            using MemoryStream docxStream = new(wordDocumentContents);
            using WordDocument wordDocument = new(docxStream, FormatType.Docx);
            using DocIORenderer renderer = new();
            using PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument);

            if (File.Exists(pdfPath))
            {
                throw new IOException($"A file with the name '{pdfPath}' already exists.");
            }

            using FileStream outputStream = new(pdfPath, FileMode.CreateNew, FileAccess.Write);
            pdfDocument.Save(outputStream);

            return pdfPath;
        }
        catch (IOException ioEx)
        {
            logger.LogError(ioEx, "An I/O error occurred while creating the PDF.");
            throw new InvalidOperationException("An I/O error occurred while creating the PDF.", ioEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the PDF document.");
            throw new ApplicationException("An error occurred while creating the PDF document.", ex);
        }
    }
}

