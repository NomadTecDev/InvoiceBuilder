using System.Globalization;
using System.Text.Json;

namespace InvoiceBuilder.Core.Entities;
public class InvoiceSettings
{
    public string SourceFile { get; init; }
    public string TemplateFile { get; init; } = null!;
    public string OutputPdfPath { get; init; } = null!;
    public JsonElement Mapping { get; init; }
    public int DefaultExpireDays { get; init; } = 14;
    public decimal DefaultVateRate { get; init; } = 21;
    public string DefaultCurrency { get; init; } = "";
    public string MappingRawString { get; set; } = null!;
}
