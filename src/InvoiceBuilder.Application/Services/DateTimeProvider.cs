using InvoiceBuilder.Core.Interfaces;

namespace InvoiceBuilder.Application.Services;

internal class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateOnly Today => DateOnly.FromDateTime(Now);
    public TimeOnly CurrentTime => TimeOnly.FromDateTime(Now);
}
