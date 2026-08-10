using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Core.Responses
{
    public class GetRegressionTailResponse
    {
        public List<DateOnly> Dates { get; set; } = [];
        public string PortfolioName { get; set; } = string.Empty;
        public List<RegressionTailData> Items { get; set; } = [];
    }

    public class RegressionTailData
    {
        public string TickerFirst { get; set; } = string.Empty;
        public string TickerSecond { get; set; } = string.Empty;
        public List<RegressionTailDataItem> Tails { get; set; } = [];
    }

    public class RegressionTailDataItem
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; } = null;
        public string ColorFill { get; set; } = string.Empty;
    }
}
