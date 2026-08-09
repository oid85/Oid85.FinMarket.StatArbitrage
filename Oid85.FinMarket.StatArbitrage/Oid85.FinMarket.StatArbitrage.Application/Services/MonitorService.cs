using Microsoft.Extensions.Options;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Core.Configuration;
using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Services
{
    public class MonitorService(
        IOptions<StatArbitrageSettings> options,
        IDataService dataService) 
        : IMonitorService
    {
        public async Task<PortfolioData> GetPortfolioDataAsync(string portfolioName, List<StrategyExecuteResult> strategyExecuteResults)
        {
            var portfolioData = new PortfolioData();

            return portfolioData;
        }
    }
}
