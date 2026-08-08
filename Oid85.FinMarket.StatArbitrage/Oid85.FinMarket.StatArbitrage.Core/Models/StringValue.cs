namespace Oid85.FinMarket.StatArbitrage.Core.Models
{
    /// <summary>
    /// Строка - значение
    /// </summary>
    public class StringValue<T>
    {
        public string Date { get; set; }
        public T Value { get; set; }
    }
}
