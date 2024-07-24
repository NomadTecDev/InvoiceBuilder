using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceBuilder.Application.Entities
{
    public record Invoice
    {
        public string Number { get; init; }
        public DateOnly Date { get; init; }
        public Company Company { get; init; }
        public DateOnly ExpireDate { get; init; }
        public string Currency { get; init; } = "EUR";
        public List<InvoiceRow> InvoiceRows { get; init; }
    }
}
