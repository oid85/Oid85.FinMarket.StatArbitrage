namespace Oid85.FinMarket.StatArbitrage.Core.Models
{
    public class RegressionTailSet
    {
        public string PortfolioName { get; set; }
        public string TickerFirst { get; set; }
        public string TickerSecond { get; set; }
        public List<DateValue<double>> Tails { get; set; } = [];
        public double Slope { get; set; }
        public double Intercept { get; set; }
    }
}
