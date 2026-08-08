namespace Oid85.FinMarket.StatArbitrage.Core.Requests
{
    public class StrategyExecuteRequest
    {
        public string? PortfolioName { get; set; }
        public string? ProcessName { get; set; }
        public bool IsOptimization { get; set; }        
    }
}
