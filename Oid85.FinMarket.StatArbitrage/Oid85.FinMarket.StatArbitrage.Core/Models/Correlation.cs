namespace Oid85.FinMarket.StatArbitrage.Core.Models;

public class Correlation
{
    public string PortfolioName { get; set; }
    public string TickerFirst { get; set; }
    public string TickerSecond { get; set; }
    public double Value { get; set; }
}