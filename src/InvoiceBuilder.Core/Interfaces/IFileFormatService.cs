using InvoiceBuilder.Core.Enums;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IFileFormatService
{   
    public FileFormat GetFileFormat(string sourceFile);
}
