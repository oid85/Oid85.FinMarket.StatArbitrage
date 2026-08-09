namespace Oid85.FinMarket.StatArbitrage.Core.Models;

public class Trade
{
    public DateOnly Date { get; set; }
    public (int First, int Second) Quantity { get; set; }
    public (double First, double Second) Price { get; set; }
    public int CandleIndex { get; set; }
}