using CsvHelper;
using InvoiceBuilder.Application.Services;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Interfaces;
using InvoiceBuilder.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Drawing.Text;
using System.Text.Json;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("invoiceSettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
        });

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        var invoiceSettings = context.Configuration.GetSection("InvoiceSettings").Get<InvoiceSettings>();
        var invoiceMapping = context.Configuration.GetSection("InvoiceSettings:Mapping").Get<JsonElement>();


        if (invoiceSettings is null)
        {
            logger.LogError("InvoiceSettings configuration section is missing or invalid.");
            throw new ArgumentNullException(nameof(invoiceSettings), "InvoiceSettings configuration section is missing or invalid.");
        }

        services.AddSingleton(invoiceSettings);
        services.AddSingleton<IInvoiceService, InvoiceService>();
        services.AddSingleton<IInvoiceRepository, CsvInvoiceRepository>();
    })
    .Build();

var invoiceService = host.Services.GetRequiredService<IInvoiceService>();
var invoice = invoiceService.GetLatestInvoice();

Console.WriteLine($"Factuurnummer {invoice.InvoiceNumber}");

