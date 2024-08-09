namespace InvoiceBuilder.Core.Entities;

internal class Invoice
{
    public string InvoiceNumber { get; init; } = null!;
    public DateOnly? InvoiceDate { get; set; }
    public Company Company { get; init; } = null!;
    public List<InvoiceRows> InvoiceRows { get; init; } = null!;
    public DateOnly? ExpireDate { get; set; }
    public int? ExpireDays { get; set; }
    public string? Currency { get; set; }
    public decimal? Subtotal { get; set; }
    public decimal? VatTotal { get; set; }
    public decimal? Total { get; set; }
    public decimal? VatRate { get; set; }
}
