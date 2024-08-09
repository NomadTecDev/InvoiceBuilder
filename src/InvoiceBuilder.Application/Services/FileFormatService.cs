using HeyRed.Mime;
using InvoiceBuilder.Core.Enums;
using InvoiceBuilder.Core.Interfaces;

namespace InvoiceBuilder.Application.Services;

internal class FileFormatService : IFileFormatService
{
    /// <summary>
    /// MimeTypesMap is a NuGet package is used to determine the file type based on it's file contents.
    /// The method returns an file extension
    /// Method of the Nuget package is inserted with dependency injection
    /// </summary>
    /// <param name="sourceFile"></param>
    /// <returns>string fileFormat</returns>
    public InputFormat GetFileFormat(string sourceFile) => MimeTypesMap.GetMimeType(sourceFile) switch
    {
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => InputFormat.Excel,
        "text/csv" => InputFormat.CSV,
        _ => throw new Exception("File format not supported")
    };
}
