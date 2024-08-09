using InvoiceBuilder.Application.Extensions;
using InvoiceBuilder.Application.UseCases.Interfaces;
using InvoiceBuilder.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Syncfusion.Compression.Zip;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddApplicationConfiguration(context.HostingEnvironment);
                services.AddSingleton<IConsole, ConsoleWrapper>(); // enables different console for testing
            })
            .Build();

        Run(host);
    }
    internal static void Run(IHost host)
    {
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var console = host.Services.GetRequiredService<IConsole>();

        try
        {
            var getLatestInvoiceUseCase = host.Services.GetRequiredService<IGetLatestInvoiceUseCase>();

            var invoice = getLatestInvoiceUseCase.Execute();
            logger.LogInformation($"Invoice number: {invoice.InvoiceNumber}");
            console.WriteLine($"Invoice number: {invoice.InvoiceNumber}");

            var generateInvoiceUseCase = host.Services.GetRequiredService<IGenerateInvoiceUseCase>();
            var filename = generateInvoiceUseCase.Execute(invoice);
            logger.LogInformation($"File generated: {filename}");
            console.WriteLine($"File generated: {filename}");

            console.WriteLine($"File {filename} genereated, please press Y to view the file now!");
            var keyInfo = console.ReadKey(intercept: true);
            switch (char.ToLower(keyInfo.KeyChar))
            {
                case 'y':
                    OpenFile(filename);
                    break;
                default:
                    console.WriteLine("Goodbye!");
                    break;
            }
        }
        catch (Exception ex)
        {
            var rootCause = ex;
            while (rootCause.InnerException is not null)
            {
                rootCause = rootCause.InnerException;
            }

            logger.LogError(rootCause, "An unexpected error occurred. Please try again later.");
            console.WriteLine($"An unexpected error occurred. Please try again later. {rootCause.Message}");
        }
    }

    private static void OpenFile(string fileName)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException(fileName, ex);
        }
    }
}
