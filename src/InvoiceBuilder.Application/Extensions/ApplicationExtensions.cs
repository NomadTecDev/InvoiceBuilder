using InvoiceBuilder.Application.Services;
using InvoiceBuilder.Core.Entities;
using InvoiceBuilder.Core.Enums;
using InvoiceBuilder.Core.Interfaces;
using InvoiceBuilder.Ifrastructure.Repositories;
using InvoiceBuilder.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Syncfusion.Licensing;
using InvoiceBuilder.Configuration.Extensions;
using InvoiceBuilder.Application.UseCases.Interfaces;
using InvoiceBuilder.Application.UseCases;

namespace InvoiceBuilder.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IHostEnvironment env)
    {
        var configuration = new ConfigurationBuilder()
            .AddAppSettingsConfiguration(env)
            .Build();

        services.AddSingleton(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger("Application");
        });

        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Application");

        var invoiceSettings = configuration.GetSection(nameof(InvoiceSettings)).Get<InvoiceSettings>();

        if (invoiceSettings == null)
        {
            logger.LogError($"{nameof(InvoiceSettings)} configuration section is missing or invalid.");
            throw new ArgumentNullException(nameof(invoiceSettings), $"{nameof(InvoiceSettings)} configuration section is missing or invalid.");
        }

        if(string.IsNullOrWhiteSpace(invoiceSettings.OutputPath) || !Directory.Exists(invoiceSettings.OutputPath))
        {
            invoiceSettings.OutputPath = Directory.GetCurrentDirectory();
        }

        logger.LogInformation($"{nameof(InvoiceSettings)} successfully read");

        var licenceKey = configuration.GetSection("Licences:Syncfusion.com:Key").Value;
        if (string.IsNullOrWhiteSpace(licenceKey))
        {
            logger.LogError("Syncfusion licence key is missing or invalid.");
            throw new ArgumentNullException(nameof(licenceKey), "Syncfusion licence key is missing or invalid.");
        }
        SyncfusionLicenseProvider.RegisterLicense(licenceKey);


        services.AddSingleton(invoiceSettings);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IConfigurationMapper, ConfigurationMapper>();
        services.AddSingleton<IFileFormatService, FileFormatService>();
        services.AddSingleton<IInvoiceRepositoryFactory, InvoiceRepositoryFactory>();
        services.AddSingleton<IInvoiceProcessor, InvoiceProcessor>();
        services.AddSingleton<IWordTemplateProcessor, WordTemplateProcessor>();

        services.AddSingleton<CsvInvoiceRepository>();
        services.AddSingleton<ExcelInvoiceRepository>();

        services.AddSingleton<DocumentGeneratorSyncfusionPDF>();
        services.AddSingleton<DocumentGeneratorWord>();

        services.AddSingleton<IInvoiceRepository>(sp =>
        {
            var factory = sp.GetRequiredService<IInvoiceRepositoryFactory>();
            return factory.GetInvoiceRepository();
        });

        services.AddSingleton<IDocumentGenerator>(sp =>
        {
            return invoiceSettings.OutputFormat switch
            {
                OutputFormat.Word => sp.GetRequiredService<DocumentGeneratorWord>(),
                _ => sp.GetRequiredService<DocumentGeneratorSyncfusionPDF>(),
            };
        });

        services.AddSingleton<IGetLatestInvoiceUseCase, GetLatestInvoiceUseCase>();
        services.AddSingleton<IGenerateInvoiceUseCase, GenerateInvoiceUseCase>();

        return services;
    }
}
