namespace Oid85.FinMarket.StatArbitrage.Core.Models;

public class Position
{
    public (string First, string Second) Ticker { get; set; }
    public (double First, double Second) EntryPrice { get; set; }
    public (double? First, double? Second) ExitPrice { get; set; } = (null, null);
    public DateOnly EntryDate { get; set; }
    public DateOnly? ExitDate { get; set; } = null;
    public int EntryCandleIndex { get; set; }
    public int? ExitCandleIndex { get; set; } = null;
    public bool IsActive { get; set; }
    public bool IsLongShort { get; set; }
    public bool IsShortLong { get; set; }
    public (int First, int Second) Quantity { get; set; }
    public double Cost { get; set; }
    public double NetProfit { get; set; }
    public double NetProfitPercent { get; set; }
    public double TotalNetProfit { get; set; }
    public double TotalProfitPct { get; set; }
}