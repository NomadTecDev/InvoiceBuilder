using InvoiceBuilder.Core.Interfaces;

namespace InvoiceBuilder.Application.Services;

internal class DateTimeProvider : IDateTimeProvider
{
    private readonly DateTime _aplicationStart = DateTime.Now;
    public DateTime Now => _aplicationStart;
    public DateOnly Today => DateOnly.FromDateTime(Now);
    public TimeOnly CurrentTime => TimeOnly.FromDateTime(Now);
}
