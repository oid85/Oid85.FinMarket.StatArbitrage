using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Repositories;
using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Repositories
{
    public class RegressionTailRepository(
        IDbContextFactory<StatArbitrageContext> contextFactory) 
        : IRegressionTailRepository
    {
        public async Task AddAsync(List<RegressionTailSet> regressionTails)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            if (regressionTails is []) return;

            var entities = regressionTails
                .Select(x => new Entities.RegressionTailSetEntity
                {
                    PortfolioName = x.PortfolioName,
                    TickerFirst = x.TickerFirst,
                    TickerSecond = x.TickerSecond,
                    Slope = x.Slope,
                    Intercept = x.Intercept,
                    Tails = JsonSerializer.Serialize(x.Tails)
                })
                .ToList();

            await context.RegressionTailSetEntities.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string portfolioName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            await context.RegressionTailSetEntities
                .Where(x => x.PortfolioName == portfolioName)
                .ExecuteDeleteAsync();

            await context.SaveChangesAsync();
        }

        public async Task<List<RegressionTailSet>> GetAsync(string portfolioName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            return [.. (await context.RegressionTailSetEntities
                .Where(x => x.PortfolioName == portfolioName)
                .AsNoTracking()
                .ToListAsync())
                .Select(x => 
                new RegressionTailSet
                {
                    PortfolioName = x.PortfolioName,
                    TickerFirst = x.TickerFirst,
                    TickerSecond = x.TickerSecond,
                    Slope = x.Slope,
                    Intercept = x.Intercept,
                    Tails = JsonSerializer.Deserialize<List<DateValue<double>>>(x.Tails)!
                })];
        }
    }
}
