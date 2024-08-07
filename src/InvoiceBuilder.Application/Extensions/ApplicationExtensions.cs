﻿using InvoiceBuilder.Application.Services;
using InvoiceBuilder.Configuration.Extensions;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Enums;
using InvoiceBuilder.Core.Interfaces;
using InvoiceBuilder.Ifrastructure.Repositories;
using InvoiceBuilder.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger("Application");
        });

        services.AddConfiguration(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var invoiceSettings = serviceProvider.GetRequiredService<InvoiceSettings>();
        services.AddSingleton(invoiceSettings);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IConfigurationMapper, ConfigurationMapper>();
        services.AddSingleton<IInvoiceService, InvoiceService>();
        services.AddSingleton<IFileFormatService, FileFormatService>();
        services.AddSingleton<IInvoiceRepositoryFactory, InvoiceRepositoryFactory>();
        services.AddSingleton<IInvoiceProcessor, InvoiceProcessor>();
        services.AddSingleton<IWordTemplateProcessor, WordTemplateProcessor>();

        services.AddSingleton<CsvInvoiceRepository>();
        services.AddSingleton<ExcelInvoiceRepository>();

        services.AddSingleton<PdfDocumentGenerator>();
        services.AddSingleton<WordDocumentGenerator>();

        services.AddSingleton<IInvoiceRepository>(sp =>
        {
            var factory = sp.GetRequiredService<IInvoiceRepositoryFactory>();
            return factory.GetInvoiceRepository();
        });

        services.AddSingleton<IDocumentGenerator>(sp =>
        {
            return invoiceSettings.OutputFormat switch
            {
                OutputFormat.Word => sp.GetRequiredService<WordDocumentGenerator>(),
                _ => sp.GetRequiredService<PdfDocumentGenerator>(),
            };
        });

        return services;
    }
}
