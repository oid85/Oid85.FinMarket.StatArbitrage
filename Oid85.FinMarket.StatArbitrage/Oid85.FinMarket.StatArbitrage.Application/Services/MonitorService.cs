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

            double money = strategyExecuteResults.Count * 1_000_000.0; // portfolioSettings!.Money;
            double totalSum = strategyExecuteResults.Count * 1_000_000.0; // portfolioSettings!.Money;

            var from = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * 365));
            var to = DateOnly.FromDateTime(DateTime.Today);
            List<DateOnly> dates = DateUtils.GetDates(from, to);

            List<string> tickersFromStrategyExecuteResults = [
                .. strategyExecuteResults.Select(x => x.TickerFirst),
                .. strategyExecuteResults.Select(x => x.TickerSecond)
                ];

            List<string> tickers = [.. tickersFromStrategyExecuteResults.Distinct()];

            var candleData = await dataService.GetCandleDataAsync(tickers);
            var instrumentData = await dataService.GetInstrumentDataAsync(tickers);

            var positions = tickers.ToDictionary(k => k, v => new PortfolioPosition());

            var lots = instrumentData.ToDictionary(k => k.Key, v => v.Value.Lot ?? 1);
            var leverages = instrumentData.ToDictionary(
                k => k.Key, 
                v => v.Value.Type == KnownInstrumentTypes.Future
                    ? portfolioSettings!.FutureLeverage
                    : portfolioSettings!.ShareLeverage);

            var positionWeightData = MonitorHelper.GetPositionWeightData(strategyExecuteResults, tickers, dates);

            var portfolioData = new PortfolioData { PositionWeightData = positionWeightData };

            foreach (var date in dates)
            {
                var weights = MonitorHelper.GetPositionWeightDataByDate(positionWeightData, date);
                var weightsSum = weights.Sum(x => Math.Abs(x.Weight));

                if (weightsSum == 0.0)
                    continue;

                double baseUnit = totalSum / (strategyExecuteResults.Count * 2);

                foreach (var ticker in tickers)
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
                            var (position, moneyChange) = PortfolioPositionHelper.CreateNewPortfolioPosition(targetWeight, targetSize, currentPrice);                            
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
                                var (position, moneyChange) = PortfolioPositionHelper.UpLongPortfolioPosition(positions[ticker], targetWeight, targetSize, currentPrice);
                                positions[ticker] = position;
                                money += moneyChange;
                            }

                            // Сократить длинную позицию
                            if (targetSize < positions[ticker].Size)
                            {
                                var (position, moneyChange) = PortfolioPositionHelper.DownLongPortfolioPosition(positions[ticker], targetWeight, targetSize, currentPrice);
                                positions[ticker] = position;
                                money += moneyChange;
                            }

                            // Не менять длинную позицию
                            if (targetSize == positions[ticker].Size)
                            {
                                // Обновим данные по позиции
                                positions[ticker].Profit = (currentPrice - positions[ticker].EntryPrice!.Value) * positions[ticker].Size;
                            }
                        }

                        // Перевернуть длинную позицию (закрыть длинную и открыть короткую позицию)
                        if (targetSize < 0)
                        {
                            var (position, moneyChange) = PortfolioPositionHelper.ReverseLongPortfolioPosition(positions[ticker], targetWeight, targetSize, currentPrice);
                            positions[ticker] = position;
                            money += moneyChange;
                        }

                        // Закрыть длинную позицию
                        if (targetSize == 0)
                        {
                            var (position, moneyChange) = PortfolioPositionHelper.CloseLongPortfolioPosition(positions[ticker], currentPrice);
                            positions[ticker] = position;
                            money += moneyChange;
                        }
                    }

                    // Короткая позиция
                    else if (positions[ticker].Size < 0)
                    {
                        // Изменить короткую позицию
                        if (targetSize < 0)
                        {
                            // Нарастить короткую позицию
                            if (targetSize < positions[ticker].Size)
                            {
                                var (position, moneyChange) = PortfolioPositionHelper.UpShortPortfolioPosition(positions[ticker], targetWeight, targetSize, currentPrice);
                                positions[ticker] = position;
                                money += moneyChange;
                            }

                            // Сократить короткую позицию
                            if (targetSize > positions[ticker].Size)
                            {
                                var (position, moneyChange) = PortfolioPositionHelper.DownShortPortfolioPosition(positions[ticker], targetWeight, targetSize, currentPrice);
                                positions[ticker] = position;
                                money += moneyChange;
                            }

                            // Не менять короткую позицию
                            if (targetSize == positions[ticker].Size)
                            {
                                // Обновим данные по позиции
                                positions[ticker].Profit = (positions[ticker].EntryPrice!.Value - currentPrice) * positions[ticker].Size;
                            }
                        }

                        // Перевернуть короткую позицию (закрыть короткую и открыть длинную позицию)
                        if (targetSize > 0)
                        {
                            var (position, moneyChange) = PortfolioPositionHelper.ReverseShortPortfolioPosition(positions[ticker], targetWeight, targetSize, currentPrice);
                            positions[ticker] = position;
                            money += moneyChange;
                        }

                        // Закрыть короткую позицию
                        if (targetSize == 0)
                        {
                            var (position, moneyChange) = PortfolioPositionHelper.CloseShortPortfolioPosition(positions[ticker], currentPrice);
                            positions[ticker] = position;
                            money += moneyChange;
                        }
                    }
                }

                double sumPositions = 0.0;

                foreach (var position in positions)
                {
                    if (position.Value.EntryPrice.HasValue)
                    {
                        double sumPosition = position.Value.EntryPrice.Value * position.Value.Size + position.Value.Profit;
                        sumPositions += sumPosition;
                    }
                }

                totalSum = sumPositions + money;

                portfolioData.MoneyCurve.Add(new DateValue<double> { Date = date, Value = money });
                portfolioData.EqiutyCurve.Add(new DateValue<double> { Date = date, Value = totalSum });

                if (portfolioData.EqiutyCurve.Count > 0)
                {
                    var maxEquity = portfolioData.EqiutyCurve.MaxBy(x => x.Value);
                    var currentEquity = portfolioData.EqiutyCurve[^1];
                    var currentDrawdown = -1 * (maxEquity!.Value - currentEquity.Value);

                    portfolioData.DrawdownCurve.Add(new DateValue<double> { Date = currentEquity.Date, Value = currentDrawdown });
                }
            }

            return portfolioData;
        }
    }
}
