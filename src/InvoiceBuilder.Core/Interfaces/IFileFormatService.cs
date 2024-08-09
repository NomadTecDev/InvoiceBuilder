using InvoiceBuilder.Core.Enums;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IFileFormatService
{   
    public InputFormat GetFileFormat(string sourceFile);
}
