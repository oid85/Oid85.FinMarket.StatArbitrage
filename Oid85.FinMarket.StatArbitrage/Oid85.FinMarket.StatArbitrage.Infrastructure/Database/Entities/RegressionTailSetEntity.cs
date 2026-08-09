using Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Entities.Base;

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Entities
{
    public class RegressionTailSetEntity : BaseEntity
    {
        public string PortfolioName { get; set; }
        public string TickerFirst { get; set; }
        public string TickerSecond { get; set; }
        public string Tails { get; set; }
        public double Slope { get; set; }
        public double Intercept { get; set; }
    }
}
