using InvoiceBuilder.Core.Enums;

namespace InvoiceBuilder.Core.Interfaces;

public interface IFileFormatService
{   
    public FileFormat GetFileFormat(string sourceFile);
}
