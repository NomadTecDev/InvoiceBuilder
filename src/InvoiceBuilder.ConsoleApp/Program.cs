using InvoiceBuilder.Application.Services;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using InvoiceBuilder.Ifrastructure.Repositories;
using InvoiceBuilder.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
var logger = loggerFactory.CreateLogger<Program>();


var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();

        var invoiceSettings = context.Configuration.GetSection("InvoiceSettings").Get<InvoiceSettings>();

        if (invoiceSettings is null)
        {
            logger.LogError("InvoiceSettings configuration section is missing or invalid.");
            throw new ArgumentNullException(nameof(invoiceSettings), "InvoiceSettings configuration section is missing or invalid.");
        }

        logger.LogInformation("InvoiceSettings successfully read");

        services.AddSingleton(invoiceSettings);

        services.AddSingleton<IInvoiceService, InvoiceService>();
        services.AddSingleton<IFileFormatService, FileFormatService>();
        services.AddSingleton<IInvoiceRepositoryFactory, InvoiceRepositoryFactory>();
        services.AddSingleton<IInvoiceMapper, InvoiceMapper>();
        services.AddSingleton<IInvoiceProcessor, InvoiceProcessor>();

        services.AddSingleton<CsvInvoiceRepository>();
        services.AddSingleton<ExcelInvoiceRepository>();

        services.AddSingleton<IInvoiceRepository>(sp =>
        {
            var factory = sp.GetRequiredService<IInvoiceRepositoryFactory>();
            return factory.GetInvoiceRepository();
        });
    })
.Build();

var invoiceService = host.Services.GetRequiredService<IInvoiceService>();
var invoice = invoiceService.GetLatestInvoice();

logger.LogInformation($"Factuurnummer {invoice.InvoiceNumber}");
Console.WriteLine($"Factuurnummer {invoice.InvoiceNumber}");

