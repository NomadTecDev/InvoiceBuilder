using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceBuilder.Core.Interfaces;
internal interface IDateTimeProvider
{
    DateTime Now { get; }
    DateOnly Today { get; }
    TimeOnly CurrentTime { get; }
}
