namespace InvoiceBuilder.Core.Entities;

public record InvoiceSettings(
    string WordTemplateFile, 
    string OutputPdfPath, 
    int DefaultExpireDays, 
    decimal DefaultVateRate
);
