using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Core.Responses
{
    public class GetRegressionTailResponse
    {
        public List<DateOnly> Dates;
        public string PortfolioName { get; set; }
        public List<RegressionTailData> Items { get; set; } = [];
    }

    public class RegressionTailData
    {
        public string TickerFirst { get; set; }
        public string TickerSecond { get; set; }
        public List<DateValue<double?>> Tails { get; set; } = [];
    }
}
