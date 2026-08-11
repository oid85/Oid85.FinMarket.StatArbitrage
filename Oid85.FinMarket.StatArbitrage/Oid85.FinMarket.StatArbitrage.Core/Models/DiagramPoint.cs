namespace Oid85.FinMarket.StatArbitrage.Core.Models;

public class DiagramPoint
{
    public int Index { get; set; }
    public DateOnly Date { get; set; }
    public double? PriceFirst { get; set; } = null;
    public double? PriceSecond { get; set; } = null;
    public double? Tail { get; set; } = null;
    public double? Indicator { get; set; } = null;
}