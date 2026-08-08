namespace Oid85.FinMarket.StatArbitrage.Core.Models
{
    /// <summary>
    /// Дата - значение
    /// </summary>
    public class DateValue<T>
    {
        public DateOnly Date { get; set; }
        public T Value { get; set; }
    }
}
