using InvoiceBuilder.Shared.Tests;
using Microsoft.Extensions.Hosting;
using Xunit;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using InvoiceBuilder.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using InvoiceBuilder.ConsoleApp;
using Xunit.Abstractions;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Moq;
using Xunit.Sdk;
using System.Text.RegularExpressions;
using InvoiceBuilder.Core.Enums;

namespace InvoiceBuilder.EndToEnd.Tests;

public class ProgramTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    internal void Program_ShouldGenerateSuccessWhenNoErrors()
    {
        var host = TestsHost.BuildHost(
            testOutputHelper,
            MockFactories.CreateGetLatestInvoiceUseCaseMock().Object,
            MockFactories.CreateGenerateInvoiceUseCaseMock().Object
            );

        // Arrange
        var testConsole = host.GetTestConsole();
        testConsole.QueueKey('N');

        // Act
        Program.Run(host);
        var consoleOutput = testConsole.GetOutput();

        // Assert
        Assert.Contains("Invoice number: TEST-001", consoleOutput);
        Assert.Contains("File generated: TEST-001.pdf", consoleOutput);
    }

    [Fact]
    internal void Program_ShouldGenerateInvoiceFile()
    {
        var host = TestsHost.BuildHost(
            testOutputHelper
            );

        // Arrange
        var testConsole = host.GetTestConsole();
        testConsole.QueueKey('N');

        // Act
        using var tempDirectory = new TempDirectory();
        var invoiceSettings = host.Services.GetRequiredService<InvoiceSettings>();
        invoiceSettings.OutputPath = tempDirectory.Path;
        Program.Run(host);
        var consoleOutput = testConsole.GetOutput();
        string invoiceNumber = Regex.Match(consoleOutput, @"Invoice number: (\w+)")?.Groups[1].Value ?? "ERROR on InvoiceNumber";
        string fileFormat = invoiceSettings.OutputFormat == OutputFormat.Word ? "docx" : "pdf";

        // Assert
        Assert.Contains($"File {tempDirectory.Path}\\{invoiceNumber}.{fileFormat} genereated", consoleOutput);
    }
}
