namespace Oid85.FinMarket.StatArbitrage.Core.Requests
{
    public class EditPortfolioTotalSumRequest
    {
        public string PortfolioName { get; set; } = string.Empty;
        public double TotalSum { get; set; }
    }
}
