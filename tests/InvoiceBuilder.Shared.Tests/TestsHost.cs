using InvoiceBuilder.Application.Extensions;
using InvoiceBuilder.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace InvoiceBuilder.Shared.Tests;

internal static class TestsHost
{
    /// <summary>
    /// Creates a test host with the default InvoiceBuilder.Configuration configuration.
    /// </summary>
    /// <returns></returns>
    public static IHost BuildHost() =>
        BuildHostInternal(null);

    /// <summary>
    /// Creates a test host with the default InvoiceBuilder.Configuration configuration and mocked services.
    /// </summary>
    /// <param name="mockedServices"></param>
    /// <returns></returns>
    public static IHost BuildHost(params object[] mockedServices) =>
        BuildHostInternal(null, mockedServices);

    /// <summary>
    /// Creates a test host with the default InvoiceBuilder.Configuration configuration and a testable console IConsole.
    /// </summary>
    /// <param name="testOutputHelper"></param>
    /// <returns></returns>
    public static IHost BuildHost(ITestOutputHelper testOutputHelper) =>
        BuildHostInternal(testOutputHelper);

    /// <summary>
    /// Creates a test host with the default InvoiceBuilder.Configuration configuration, mocked services and a testable console IConsole.
    /// </summary>
    /// <param name="mockedServices"></param>
    /// <param name="testOutputHelper"></param>
    /// <returns></returns>
    public static IHost BuildHost(ITestOutputHelper testOutputHelper, params object[] mockedServices) =>
        BuildHostInternal(testOutputHelper, mockedServices);

    private static IHost BuildHostInternal(ITestOutputHelper? testOutputHelper = null, params object[] mockedServices) =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddApplicationConfiguration(new FakeHostingEnvironment());

                if (testOutputHelper is not null)
                {
                    services.AddSingleton<IConsole>(new TestConsole(testOutputHelper));
                }

                if (mockedServices is not null)
                {
                    foreach (var mock in mockedServices)
                    {
                        var serviceType = mock.GetType().GetInterfaces().First();
                        services.AddSingleton(serviceType, mock);
                    }
                }
            })
            .Build();

    private class FakeHostingEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "InvoiceBuilder.Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
