using InvoiceBuilder.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvoiceBuilder.Configuration.Extensions;

internal static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder config, IHostEnvironment env)
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();

        return config;
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);

        services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Configuration");
            var invoiceSettings = configuration.GetSection(nameof(InvoiceSettings)).Get<InvoiceSettings>();

            if (invoiceSettings == null)
            {
                logger.LogError($"{nameof(InvoiceSettings)} configuration section is missing or invalid.");
                throw new ArgumentNullException(nameof(invoiceSettings), $"{nameof(InvoiceSettings)} configuration section is missing or invalid.");
            }

            logger.LogInformation($"{nameof(InvoiceSettings)} successfully read");
            return invoiceSettings;
        });

        return services;
    }

    private static object GetSectionData(IConfigurationSection section)
    {
        var children = section.GetChildren().ToList();
        if (!children.Any())
        {
            return section.Value;
        }

        var result = new List<Dictionary<string, object>>();
        foreach (var child in children)
        {
            var childData = new Dictionary<string, object>
            {
                { child.Key, GetSectionData(child) }
            };
            result.Add(childData);
        }

        return result;
    }
}
