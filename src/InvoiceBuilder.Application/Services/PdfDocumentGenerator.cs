using DocumentFormat.OpenXml.Packaging;
using InvoiceBuilder.Core.Enums;
using InvoiceBuilder.Core.Interfaces;
using OpenXmlPowerTools;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Xml.Linq;

namespace InvoiceBuilder.Application.Services;

internal class PdfDocumentGenerator() : IDocumentGenerator
{
    private static readonly PdfOptions _pdfOptions = new() { Landscape = false, Format = PaperFormat.A4 };

    public string Create(byte[] wordDocumentContents, string outputPath, string documentName)
    {
        using var memoryStream = new MemoryStream();

        memoryStream.Write(wordDocumentContents, 0, wordDocumentContents.Length);

        // memoryStream.Position = 0;

        using var wordprocessingDocument = WordprocessingDocument.Open(memoryStream, true);

        var settings = new HtmlConverterSettings()
        {
            PageTitle = documentName
        };

        XElement htmlElement = HtmlConverter.ConvertToHtml(wordprocessingDocument, settings);

        string htmlString = htmlElement.ToString();

        return ConvertHtmlToPdf(htmlString, Path.Combine(outputPath, $"{documentName}.pdf") );
    }

    static string ConvertHtmlToPdf(string htmlString, string pdfPath)
    {
        var browserFetcher = new BrowserFetcher();
        var revisionInfo = browserFetcher.DownloadAsync().Result;

        using var browser = Puppeteer.LaunchAsync(new LaunchOptions { Headless = true, ExecutablePath = revisionInfo.GetExecutablePath() }).Result;
        using var page = browser.NewPageAsync().Result;

        page.SetContentAsync(htmlString).Wait();
        page.PdfAsync(pdfPath, _pdfOptions).Wait();

        return pdfPath;
    }
}
