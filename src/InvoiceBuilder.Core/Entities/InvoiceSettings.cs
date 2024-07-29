using Microsoft.Extensions.Configuration;

namespace InvoiceBuilder.Core.Entities;
public class InvoiceSettings
{
    public string SourceFile { get; init; } = null!;
    public string TemplateFile { get; init; } = null!;
    public string OutputPdfPath { get; init; } = null!;
    public int DefaultExpireDays { get; init; } = 14;
    public decimal DefaultVateRate { get; init; } = 21;
    public string DefaultCurrency { get; init; } = "EUR";
    public string DefaultDateFormat { get; init; } = "dd/MM/yyyy";
    public IConfigurationSection SourceMapping { get; init; } = null!;
    public IConfigurationSection OutputMapping { get; init; } = null!;
}
