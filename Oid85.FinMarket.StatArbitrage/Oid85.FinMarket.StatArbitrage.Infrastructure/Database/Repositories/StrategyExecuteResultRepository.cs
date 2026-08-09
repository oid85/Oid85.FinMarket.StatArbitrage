using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Repositories;
using Oid85.FinMarket.StatArbitrage.Core.Configuration;
using Oid85.FinMarket.StatArbitrage.Core.Models;
using Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Entities;
using static Grpc.Core.Metadata;

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Repositories
{
    public class StrategyExecuteResultRepository(
        IOptions<StatArbitrageSettings> options,
        IDbContextFactory<StatArbitrageContext> contextFactory) 
        : IStrategyExecuteResultRepository
    {
        public async Task AddAsync(List<StrategyExecuteResult> strategyExecuteResults)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            if (strategyExecuteResults is []) return;

            var entities = strategyExecuteResults.Select(Map);

            await context.StrategyExecuteResultEntities.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string portfolioName, string processName)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            await context.StrategyExecuteResultEntities
                .Where(x => x.PortfolioName == portfolioName)
                .Where(x => x.ProcessName == processName)
                .ExecuteDeleteAsync();

            await context.SaveChangesAsync();
        }

        public async Task<List<StrategyExecuteResult>> GetFilteredAsync()
        {
            var statArbitrageSettings = options.Value;

            await using var context = await contextFactory.CreateDbContextAsync();

            var queryableEntities = context.StrategyExecuteResultEntities.AsQueryable();

            queryableEntities = queryableEntities.Where(x => x.ProfitFactor >= statArbitrageSettings.StrategyExecuteResultFilter.MinProfitFactor);
            queryableEntities = queryableEntities.Where(x => x.RecoveryFactor >= statArbitrageSettings.StrategyExecuteResultFilter.MinRecoveryFactor);
            queryableEntities = queryableEntities.Where(x => x.WinningTradesPercent >= statArbitrageSettings.StrategyExecuteResultFilter.MinWinningTradesPercent);
            queryableEntities = queryableEntities.Where(x => x.WinningTradesPercent <= statArbitrageSettings.StrategyExecuteResultFilter.MaxWinningTradesPercent);
            queryableEntities = queryableEntities.Where(x => x.AnnualYieldReturn >= statArbitrageSettings.StrategyExecuteResultFilter.MinAnnualYieldReturn);
            queryableEntities = queryableEntities.Where(x => x.MaxDrawdownPercent <= statArbitrageSettings.StrategyExecuteResultFilter.MaxDrawdownPercent);

            var entities = await queryableEntities.AsNoTracking().ToListAsync();

            var models = entities.Select(Map).ToList();

            return models;
        }

        private static StrategyExecuteResult Map(StrategyExecuteResultEntity entity) => 
            new()
            {
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                TickerFirst = entity.TickerFirst,
                TickerSecond = entity.TickerSecond,
                StrategyDescription = entity.StrategyDescription,
                PortfolioName = entity.PortfolioName,
                ProcessName = entity.ProcessName,
                StrategyName = entity.StrategyName,
                StrategyParams = entity.StrategyParams,
                StrategyParamsHash = entity.StrategyParamsHash,
                NumberPositions = entity.NumberPositions,
                CurrentPositionFirst = entity.CurrentPositionFirst,
                CurrentPositionSecond = entity.CurrentPositionSecond,
                CurrentPositionCost = entity.CurrentPositionCost,
                ProfitFactor = entity.ProfitFactor,
                RecoveryFactor = entity.RecoveryFactor,
                TotalNetProfit = entity.NetProfit,
                AverageNetProfit = entity.AverageNetProfit,
                AverageNetProfitPercent = entity.AverageNetProfitPercent,
                Drawdown = entity.Drawdown,
                MaxDrawdown = entity.MaxDrawdown,
                MaxDrawdownPercent = entity.MaxDrawdownPercent,
                WinningPositions = entity.WinningPositions,
                WinningTradesPercent = entity.WinningTradesPercent,
                StartMoney = entity.StartMoney,
                EndMoney = entity.EndMoney,
                TotalReturn = entity.TotalReturn,
                AnnualYieldReturn = entity.AnnualYieldReturn,
                ResultMessage = entity.ResultMessage
            };

        private static StrategyExecuteResultEntity Map(StrategyExecuteResult model) =>
            new()
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                TickerFirst = model.TickerFirst,
                TickerSecond = model.TickerSecond,
                StrategyDescription = model.StrategyDescription,
                PortfolioName = model.PortfolioName,
                ProcessName = model.ProcessName,
                StrategyName = model.StrategyName,
                StrategyParams = model.StrategyParams,
                StrategyParamsHash = model.StrategyParamsHash,
                NumberPositions = model.NumberPositions,
                CurrentPositionFirst = model.CurrentPositionFirst,
                CurrentPositionSecond = model.CurrentPositionSecond,
                CurrentPositionCost = model.CurrentPositionCost,
                ProfitFactor = model.ProfitFactor,
                RecoveryFactor = model.RecoveryFactor,
                NetProfit = model.TotalNetProfit,
                AverageNetProfit = model.AverageNetProfit,
                AverageNetProfitPercent = model.AverageNetProfitPercent,
                Drawdown = model.Drawdown,
                MaxDrawdown = model.MaxDrawdown,
                MaxDrawdownPercent = model.MaxDrawdownPercent,
                WinningPositions = model.WinningPositions,
                WinningTradesPercent = model.WinningTradesPercent,
                StartMoney = model.StartMoney,
                EndMoney = model.EndMoney,
                TotalReturn = model.TotalReturn,
                AnnualYieldReturn = model.AnnualYieldReturn,
                ResultMessage = model.ResultMessage
            };
    }
}
