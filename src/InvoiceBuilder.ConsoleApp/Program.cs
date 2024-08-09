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


var invoice = invoiceService.GetLatestInvoice();
logger.LogInformation($"Invoice number: {invoice.InvoiceNumber}");
Console.WriteLine($"Invoice number: {invoice.InvoiceNumber}");

var filename = invoiceService.GenerateInvoice(invoice);
logger.LogInformation($"File generated: {filename}");
Console.WriteLine($"File generated: {filename}");
Console.WriteLine($"Press any key to quit.");

Console.ReadKey();