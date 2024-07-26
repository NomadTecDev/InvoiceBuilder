namespace InvoiceBuilder.Core.Entities;

public class Invoice
{
    public string? InvoiceNumber { get; init; }
    public DateOnly? Date { get; init; }
    public Company? Company { get; init; }
    public DateOnly? ExpireDate { get; init; }
    public List<InvoiceRow>? InvoiceRows { get; init; }
    public string? Currency { get; set; }
    public decimal? Subtal { get; set; }
    public decimal? VatTotal { get; set; }
    public decimal? Total { get; set; }
}
