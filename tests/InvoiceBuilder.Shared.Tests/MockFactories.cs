using InvoiceBuilder.Application.UseCases.Interfaces;
using InvoiceBuilder.Core.Entities;
using Moq;

namespace InvoiceBuilder.Shared.Tests;

internal static class MockFactories
{
    internal static Mock<IGetLatestInvoiceUseCase> CreateGetLatestInvoiceUseCaseMock()
    {
        var mock = new Mock<IGetLatestInvoiceUseCase>();
        mock.Setup(m => m.Execute())
            .Returns(new Invoice { InvoiceNumber = "TEST-001" });
        return mock;
    }

    internal static Mock<IGenerateInvoiceUseCase> CreateGenerateInvoiceUseCaseMock()
    {
        var mock = new Mock<IGenerateInvoiceUseCase>();

        mock.Setup(m => m.Execute(It.IsAny<Invoice>()))
            .Returns("TEST-001.pdf");

        return mock;
    }
}