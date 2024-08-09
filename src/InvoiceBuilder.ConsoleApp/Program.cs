using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using InvoiceBuilder.Application.Extensions;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationConfiguration(context.Configuration);
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var invoiceService = host.Services.GetRequiredService<IInvoiceService>();
var invoiceSettings = host.Services.GetRequiredService<InvoiceSettings>();
var configurationMapper = host.Services.GetRequiredService<IConfigurationMapper>();

var invoice = invoiceService.GetLatestInvoice();

logger.LogInformation($"Factuurnummer {invoice.InvoiceNumber}");
Console.WriteLine($"Factuurnummer {invoice.InvoiceNumber}");

var documentVariables = configurationMapper.GetKeyValuePairs(invoiceSettings.OutputMapping, invoice);






/*
var invoiceRepository = host.Services.GetRequiredService<IInvoiceRepository>();


var wordDocumentProcessor = host.Services.GetRequiredService<IWordDocumentProcessor>();
var fileGenerator = host.Services.GetRequiredService<IFileGenerator>();

var rawData = invoiceRepository.GetRawData(invoiceSettings.SourceFile);
var invoice = configurationMapper.MapToEntity<Invoice>(invoiceSettings.SourceMapping, rawData);




var wordContents = wordDocumentProcessor.Create(invoiceSettings.TemplateFile, invoiceRepository.GetRawData(invoiceSettings.SourceFile));
fileGenerator.Create(wordContents, "mynewinvoice");




*/