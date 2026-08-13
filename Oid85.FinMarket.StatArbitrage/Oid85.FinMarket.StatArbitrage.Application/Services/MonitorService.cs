using Microsoft.Extensions.Options;
using Oid85.FinMarket.StatArbitrage.Application.Helpers;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Common.KnownConstants;
using Oid85.FinMarket.StatArbitrage.Common.Utils;
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
            var statArbitrageSettings = options.Value;
            var portfolioSettings = statArbitrageSettings.Portfolios.Find(x => x.Name == portfolioName);

            if (strategyExecuteResults is [])
                return new();

            double money = portfolioSettings!.Money;
            double totalSum = portfolioSettings!.Money;

            var from = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * 365));
            var to = DateOnly.FromDateTime(DateTime.Today);
            List<DateOnly> dates = DateUtils.GetDates(from, to);

            List<string> tickersFromStrategyExecuteResults = [
                .. strategyExecuteResults.Select(x => x.TickerFirst),
                .. strategyExecuteResults.Select(x => x.TickerSecond),
                KnownTickers.TMON];

            List<string> tickers = [.. tickersFromStrategyExecuteResults.Distinct()];

            var candleData = await dataService.GetCandleDataAsync(tickers);
            var instrumentData = await dataService.GetInstrumentDataAsync(tickers);

            var positions = tickers.ToDictionary(k => k, v => new PortfolioPosition());

            var lots = instrumentData.ToDictionary(k => k.Key, v => v.Value.Lot ?? 1);
            var leverages = instrumentData.ToDictionary(
                k => k.Key, 
                v => v.Value.Type == KnownInstrumentTypes.Future
                    ? portfolioSettings.FutureLeverage
                    : portfolioSettings.ShareLeverage);

            var positionWeightData = MonitorHelper.GetPositionWeightData(strategyExecuteResults, tickers, dates);

            var portfolioData = new PortfolioData { PositionWeightData = positionWeightData };

            foreach (var date in dates)
            {
                var weights = MonitorHelper.GetPositionWeightDataByDate(positionWeightData, date);
                var weightsSum = weights.Sum(x => Math.Abs(x.Weight));

                if (weightsSum == 0.0)
                    continue;

                double baseUnit = totalSum / strategyExecuteResults.Count;

                foreach (var ticker in tickers.Where(x => x != KnownTickers.TMON))
                {
                    double currentPrice = dataService.GetPrice(ticker, date) ?? 0.0;

                    if (currentPrice == 0.0) continue;

                    int targetWeight = weights.Find(x => x.Ticker == ticker)?.Weight ?? 0;
                    double targetCost = baseUnit * Math.Abs(targetWeight);
                    int targetSize = Convert.ToInt32(Math.Truncate((targetCost / currentPrice) / lots[ticker]) * lots[ticker]);
                    
                    // Нет позиции
                    if (positions[ticker].Weight == 0)
                    {
                        // Создать новую позицию
                        if (targetSize != 0)
                        {
                            var (position, moneyChange) = PortfolioPositionHelper.CreateNewPortfolioPosition(targetWeight, currentPrice, targetSize);                            
                            positions[ticker] = position;
                            money += moneyChange;
                        }
                    } 
                    
                    // Длинная позиция
                    else if (positions[ticker].Size > 0)
                    {
                        // Изменить длинную позицию
                        if (targetSize > 0)
                        {
                            // Нарастить длинную позицию
                            if (targetSize > positions[ticker].Size)
                            {
                                var (position, moneyChange) = PortfolioPositionHelper.UpLongPortfolioPosition(positions[ticker], targetWeight, currentPrice, targetSize);
                                positions[ticker] = position;
                                money += moneyChange;
                            }

                            // Сократить длинную позицию
                            if (targetSize < positions[ticker].Size)
                            {
                                var (position, moneyChange) = PortfolioPositionHelper.DownLongPortfolioPosition(positions[ticker], targetWeight, currentPrice, targetSize);
                                positions[ticker] = position;
                                money += moneyChange;
                            }

                            // Не менять длинную позицию
                            if (targetSize == positions[ticker].Size)
                            {
                                // Обновим данные по позиции
                                positions[ticker].Cost = currentPrice * targetSize;
                                positions[ticker].Profit = (currentPrice - positions[ticker].EntryPrice!.Value) * targetSize;
                            }
                        }

                        // Перевернуть длинную позицию (закрыть и открыть короткую позицию)
                        if (targetSize < 0)
                        {

                        }

                        // Закрыть длинную позицию
                        if (targetSize == 0)
                        {

                        }
                    }

                    // Короткая позиция
                    else if (positions[ticker].Size < 0)
                    {

                    }
                }

                portfolioData.MoneyCurve.Add(new DateValue<double> { Date = date, Value = money });
                portfolioData.EqiutyCurve.Add(new DateValue<double> { Date = date, Value = totalSum });
                portfolioData.DrawdownCurve.Add(new DateValue<double> { Date = date, Value = 0.0 });
            }

            return portfolioData;
        }
    }
}
