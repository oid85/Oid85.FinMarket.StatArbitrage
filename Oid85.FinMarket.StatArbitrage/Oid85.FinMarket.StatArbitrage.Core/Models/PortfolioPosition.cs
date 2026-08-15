namespace Oid85.FinMarket.StatArbitrage.Core.Models
{
    public class PortfolioPosition
    {
        public double? EntryPrice { get; set; } = null;
        public int Weight { get; set; } = 0;
        public int Size { get; set; } = 0;
        public double Profit { get; set; } = 0.0;
    }
}
