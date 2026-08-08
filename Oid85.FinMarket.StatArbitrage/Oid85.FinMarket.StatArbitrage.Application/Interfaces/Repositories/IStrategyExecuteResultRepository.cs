using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Interfaces.Repositories
{
    public interface IStrategyExecuteResultRepository
    {
        Task AddAsync(List<StrategyExecuteResult> strategyExecuteResults);
        Task<List<StrategyExecuteResult>> GetFilteredAsync();
        Task DeleteAsync(string portfolioName, string processName);
    }
}
