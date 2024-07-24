using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceBuilder.Application.Entities
{
    public record InvoiceRow
    {
        public string Description { get; init; } = ""
        public decimal? VatRate { get; init; }
        public decimal? Cost { get; init; }
    }
}
