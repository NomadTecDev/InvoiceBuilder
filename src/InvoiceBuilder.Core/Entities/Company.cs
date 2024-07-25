namespace InvoiceBuilder.Core.Entities;

public record Company(
    string Name,
    string ContactName,
    string Address,
    string Postal,
    string City,
    string Country,
    string ChamberOfCommerce,
    string VATID
);