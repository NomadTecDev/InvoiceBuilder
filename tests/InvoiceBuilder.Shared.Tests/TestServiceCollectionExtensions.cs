using InvoiceBuilder.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace InvoiceBuilder.Shared.Tests;

internal static class TestServiceCollectionExtensions
{
    internal static IServiceCollection AddMockedServices(this IServiceCollection services)
    {
        services.AddSingleton(MockFactories.CreateGetLatestInvoiceUseCaseMock().Object);
        services.AddSingleton(MockFactories.CreateGenerateInvoiceUseCaseMock().Object);

        return services;
    }

    internal static TestConsole GetTestConsole(this IHost host) =>
        host.Services.GetRequiredService<IConsole>() as TestConsole ?? throw new InvalidOperationException("TestConsole not found in services");
}
