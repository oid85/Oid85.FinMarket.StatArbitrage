namespace Oid85.FinMarket.StatArbitrage.Core.Models;

public class Trade
{
    public DateOnly Date { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public int CandleIndex { get; set; }
}