using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services
{
    public interface IMonitorService
    {
        Task<PortfolioData> GetPortfolioDataAsync(string portfolioName, List<StrategyExecuteResult> strategyExecuteResults);
    }
}
