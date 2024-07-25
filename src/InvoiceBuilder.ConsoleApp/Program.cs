using InvoiceBuilder.Application.Entities;
using InvoiceBuilder.Application.Interfaces;
using InvoiceBuilder.Application.Services;
using InvoiceBuilder.Application.UseCases;
using InvoiceBuilder.Application.DataSources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<InvoiceSettings>(context.Configuration.GetSection("InvoiceSettings"));

        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
        });

        services.AddTransient<IInvoiceDataSource, CsvInvoiceDataSource>(); // Or use ExcelInvoiceDataSource
        services.AddTransient<IGetLatestInvoiceUseCase, GetLatestInvoiceUseCase>();
        services.AddTransient<IGenerateInvoiceUseCase, GenerateInvoiceUseCase>();
        services.AddTransient<IInvoiceService, InvoiceService>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting application");

var invoiceService = host.Services.GetRequiredService<IInvoiceService>();

// Load configuration settings
var config = host.Services.GetRequiredService<IConfiguration>();
var filePath = config["InvoiceSettings:FilePath"];
var wordTemplatePath = config["InvoiceSettings:WordTemplatePath"];
var outputPdfPath = config["InvoiceSettings:OutputPdfPath"];

try
{
    var invoice = invoiceService.GetLatestInvoice(filePath);
    if (invoice != null)
    {
        invoiceService.GenerateInvoiceDocument(invoice, wordTemplatePath, outputPdfPath);
    }
    else
    {
        logger.LogWarning("No invoice data found to generate the document.");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while processing the invoice.");
}

logger.LogInformation("Application finished");
