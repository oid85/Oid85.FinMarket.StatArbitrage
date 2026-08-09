using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Interfaces.Repositories
{
    public interface IRegressionTailRepository
    {
        Task AddAsync(List<RegressionTailSet> regressionTails);
        Task<List<RegressionTailSet>> GetAsync(string portfolioName);
        Task DeleteAsync(string portfolioName);
    }
}
