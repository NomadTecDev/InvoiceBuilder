using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceBuilder.Application.DTOs
{
    public record ExcelInvoiceDto
    {
        public string InvoiceNumber { get; init; } = ""
        public DateOnly InvoiceDate { get; init; }
        public DateOnly ExpireDate { get; init; }
        public string Currency { get; init; } = "EUR"
        public string CompanyName { get; init; } = ""
        public string ContactName { get; init; } = ""
        public string Address { get; init; } = ""
        public string Postal { get; init; } = ""
        public string City { get; init; } = ""
        public string Country { get; init; } = ""
        public string ChamberOfCommerce { get; init; } = ""
        public string VATID { get; init; } = ""
        public string Row1Description { get; init; } = ""
        public decimal Row1VatRate { get; init; }
        public decimal Row1Cost { get; init; }
        public string Row2Description { get; init; } = ""
        public decimal Row2VatRate { get; init; }
        public decimal Row2Cost { get; init; }
        public string Row3Description { get; init; } = ""
        public decimal Row3VatRate { get; init; }
        public decimal Row3Cost { get; init; }
        public string Row4Description { get; init; } = ""
        public decimal Row4VatRate { get; init; }
        public decimal Row4Cost { get; init; }
        public string Row5Description { get; init; } = ""
        public decimal Row5VatRate { get; init; }
        public decimal Row5Cost { get; init; }
        public string Row6Description { get; init; } = ""
        public decimal Row6VatRate { get; init; }
        public decimal Row6Cost { get; init; }
    }
}
