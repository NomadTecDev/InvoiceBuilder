using InvoiceBuilder.Application.Entities;
using InvoiceBuilder.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Word;

namespace InvoiceBuilder.Application.UseCases
{
    public class GenerateInvoiceUseCase : IGenerateInvoiceUseCase
    {
        private readonly ILogger<GenerateInvoiceUseCase> _logger;

        public GenerateInvoiceUseCase(ILogger<GenerateInvoiceUseCase> logger)
        {
            _logger = logger;
        }

        public void Execute(Invoice invoice, string wordTemplatePath, string outputPdfPath)
        {
            try
            {
                var wordApp = new Application();
                var document = wordApp.Documents.Open(wordTemplatePath);

                // Replace document variables with invoice details
                foreach (DocumentVariable variable in document.Variables)
                {
                    switch (variable.Name)
                    {
                        case "InvoiceNumber":
                            variable.Value = invoice.Number;
                            break;
                        case "InvoiceDate":
                            variable.Value = invoice.Date.ToString("yyyy-MM-dd");
                            break;
                        case "ExpireDate":
                            variable.Value = invoice.ExpireDate.ToString("yyyy-MM-dd");
                            break;
                        case "Currency":
                            variable.Value = invoice.Currency;
                            break;
                        case "CompanyName":
                            variable.Value = invoice.Company.Name;
                            break;
                        case "ContactName":
                            variable.Value = invoice.Company.ContactName;
                            break;
                        case "Address":
                            variable.Value = invoice.Company.Address;
                            break;
                        case "Postal":
                            variable.Value = invoice.Company.Postal;
                            break;
                        case "City":
                            variable.Value = invoice.Company.City;
                            break;
                        case "Country":
                            variable.Value = invoice.Company.Country;
                            break;
                        case "ChamberOfCommerce":
                            variable.Value = invoice.Company.ChamberOfCommerce;
                            break;
                        case "Row1Description":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(0)?.Description;
                            break;
                        case "Row1VatRate":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(0)?.VatRate?.ToString("F2");
                            break;
                        case "Row1Cost":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(0)?.Cost?.ToString("F2");
                            break;
                        case "Row2Description":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(1)?.Description;
                            break;
                        case "Row2VatRate":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(1)?.VatRate?.ToString("F2");
                            break;
                        case "Row2Cost":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(1)?.Cost?.ToString("F2");
                            break;
                        case "Row3Description":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(2)?.Description;
                            break;
                        case "Row3VatRate":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(2)?.VatRate?.ToString("F2");
                            break;
                        case "Row3Cost":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(2)?.Cost?.ToString("F2");
                            break;
                        case "Row4Description":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(3)?.Description;
                            break;
                        case "Row4VatRate":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(3)?.VatRate?.ToString("F2");
                            break;
                        case "Row4Cost":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(3)?.Cost?.ToString("F2");
                            break;
                        case "Row5Description":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(4)?.Description;
                            break;
                        case "Row5VatRate":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(4)?.VatRate?.ToString("F2");
                            break;
                        case "Row5Cost":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(4)?.Cost?.ToString("F2");
                            break;
                        case "Row6Description":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(5)?.Description;
                            break;
                        case "Row6VatRate":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(5)?.VatRate?.ToString("F2");
                            break;
                        case "Row6Cost":
                            variable.Value = invoice.InvoiceRows.ElementAtOrDefault(5)?.Cost?.ToString("F2");
                            break;
                        case "SubTotal":
                            variable.Value = InvoiceCalculator.CalculateSubTotal(invoice).ToString("F2");
                            break;
                        case "VatTotal":
                            variable.Value = InvoiceCalculator.CalculateVatTotal(invoice).ToString("F2");
                            break;
                        case "Total":
                            variable.Value = InvoiceCalculator.CalculateTotal(invoice).ToString("F2");
                            break;
                    }
                }

                // Save the document as a PDF
                document.SaveAs2(outputPdfPath, WdSaveFormat.wdFormatPDF);
                document.Close();
                wordApp.Quit();

                _logger.LogInformation("Invoice document generated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the invoice document.");
                throw;
            }
        }
    }
}
