using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Repositories;
using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Repositories
{
    public class CorrelationRepository(
        IDbContextFactory<StatArbitrageContext> contextFactory) 
        : ICorrelationRepository
    {
        public async Task AddAsync(List<Correlation> correlations)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            if (correlations is []) return;

            var entities = correlations
                .Select(x => new Entities.CorrelationEntity
                {
                    PortfolioName = x.PortfolioName,
                    TickerFirst = x.TickerFirst,
                    TickerSecond = x.TickerSecond,
                    Value = x.Value
                })
                .ToList();

            await context.CorrelationEntities.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string portfolioName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            await context.CorrelationEntities
                .Where(x => x.PortfolioName == portfolioName)
                .ExecuteDeleteAsync();

            await context.SaveChangesAsync();
        }
    }
}
