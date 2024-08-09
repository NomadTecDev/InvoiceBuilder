using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace InvoiceBuilder.Configuration.Extensions;

internal static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddAppSettingsConfiguration(this IConfigurationBuilder config, IHostEnvironment env)
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();

        return config;
    }
}
