using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Enums;
using InvoiceBuilder.Core.Interfaces;
using InvoiceBuilder.Ifrastructure.Repositories;
using InvoiceBuilder.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceBuilder.Application.Services;

internal class InvoiceRepositoryFactory(
    IServiceProvider serviceProvider, 
    IFileFormatService fileFormatService, 
    InvoiceSettings invoiceSettings) : IInvoiceRepositoryFactory
{
    /// <summary>
    /// GetFileFormat from FileFormatService gets inserted.
    /// Based on the file type, the appropriate repository is added to the services collection.
    /// The services collection is also inserted into the method.
    /// </summary>
    /// <param name="sourceFile">The source file path.</param>
    /// <returns>The source repository.</returns>
    public IInvoiceRepository GetInvoiceRepository() => 
        fileFormatService.GetFileFormat(invoiceSettings.SourceFile) switch
        {
            FileFormat.Excel => serviceProvider.GetRequiredService<ExcelInvoiceRepository>(),
            _ => serviceProvider.GetRequiredService<CsvInvoiceRepository>(),
        };
}
