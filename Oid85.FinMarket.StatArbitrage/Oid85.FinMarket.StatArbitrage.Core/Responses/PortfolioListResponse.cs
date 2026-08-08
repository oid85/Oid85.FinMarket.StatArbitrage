namespace Oid85.FinMarket.StatArbitrage.Core.Responses
{
    public class PortfolioListResponse
    {
        public List<PortfolioListItem> Items { get; set; } = [];
    }

    public class PortfolioListItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
