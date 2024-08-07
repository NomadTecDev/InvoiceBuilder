﻿namespace InvoiceBuilder.Core.Entities;

internal class InvoiceRows
{
    public string Description { get; init; } = null!;
    public decimal? VatRate { get; set; }
    public decimal? Cost { get; init; }
}
