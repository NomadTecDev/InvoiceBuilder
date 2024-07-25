namespace InvoiceBuilder.Core.Entities;

public record InvoiceRow(
        string Description,
        decimal? VatRate,
        decimal? Cost
);
