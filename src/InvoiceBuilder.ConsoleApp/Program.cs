using InvoiceBuilder.Application.Extensions;
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

logger.LogInformation($"Factuurnummer {invoice.InvoiceNumber}");
Console.WriteLine($"Factuurnummer {invoice.InvoiceNumber}");

