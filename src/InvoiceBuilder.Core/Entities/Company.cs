namespace InvoiceBuilder.Core.Entities;

public class Company
{
    public string? Name { get; init; }
    public string? ContactName { get; init; }
    public string? Address { get; init; }
    public string? Postal { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public string? ChamberOfCommerce { get; init; }
    public string? VATID { get; init; }
}