namespace InvoiceBuilder.Core.Entities;

public record Invoice(
    string Number,
    DateOnly Date,
    Company Company,
    DateOnly ExpireDate,
    List<InvoiceRow> InvoiceRows,
    string Currency = "EUR"
);