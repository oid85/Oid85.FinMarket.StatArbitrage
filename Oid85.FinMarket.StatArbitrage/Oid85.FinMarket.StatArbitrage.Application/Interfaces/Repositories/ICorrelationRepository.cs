using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Interfaces.Repositories
{
    public interface ICorrelationRepository
    {
        Task AddAsync(List<Correlation> correlations);
        Task DeleteAsync(string portfolioName);
    }
}
