﻿using InvoiceBuilder.Core.Entities;

namespace InvoiceBuilder.Core.Interfaces;

public interface IInvoiceService
{
    Invoice GetLatestInvoice();
}
