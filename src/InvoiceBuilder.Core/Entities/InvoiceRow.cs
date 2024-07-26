namespace InvoiceBuilder.Core.Entities;

public class InvoiceRow {
    public string Description { get; init; } = null!;
    public decimal? VatRate { get; set; }
    public decimal? Cost { get; init; }
}
