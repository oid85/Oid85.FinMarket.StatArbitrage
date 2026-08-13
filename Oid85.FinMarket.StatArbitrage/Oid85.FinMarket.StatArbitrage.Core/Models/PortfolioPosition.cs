namespace Oid85.FinMarket.StatArbitrage.Core.Models
{
    public class PortfolioPosition
    {
        public double? EntryPrice { get; set; } = null;
        public bool IsActive { get; set; } = false;
        public bool IsLong { get; set; } = false;
        public bool IsShort { get; set; } = false;
        public int Weight { get; set; } = 0;
        public int Size { get; set; } = 0;
        public double Cost { get; set; } = 0.0;
        public double Profit { get; set; } = 0.0;
    }
}
