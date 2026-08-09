using Microsoft.Extensions.Options;
using NLog;
using Oid85.FinMarket.StatArbitrage.Application.Helpers;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Repositories;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Common.KnownConstants;
using Oid85.FinMarket.StatArbitrage.Common.Utils;
using Oid85.FinMarket.StatArbitrage.Core.Configuration;
using Oid85.FinMarket.StatArbitrage.Core.Models;
using Oid85.FinMarket.StatArbitrage.Core.Requests;
using Oid85.FinMarket.StatArbitrage.Core.Responses;

namespace Oid85.FinMarket.StatArbitrage.Application.Services
{
    public class CorrelationService(
        ILogger logger,
        IOptions<StatArbitrageSettings> options,
        IDataService dataService,
        ICorrelationRepository correlationRepository) 
        : ICorrelationService
    {
        public async Task<CalculateCorrelationResponse> CalculateCorrelationAsync(CalculateCorrelationRequest request)
        {
            var statArbitrageSettings = options.Value;
            var portfolioSettingsList = statArbitrageSettings.Portfolios;

            if (!string.IsNullOrEmpty(request.PortfolioName))
                portfolioSettingsList = [.. portfolioSettingsList.Where(x => x.Name == request.PortfolioName)];

            var (from, to) = StatArbitrageHelper.GetOptimizationDates(options.Value);

            foreach (var portfolioSetting in portfolioSettingsList)
            {
                await correlationRepository.DeleteAsync(portfolioSetting.Name);

                var tickers = statArbitrageSettings.TickerLists.Find(x => x.Name == portfolioSetting!.TickerList)!.Tickers;

                var candleData = (await dataService.GetCandleDataAsync(tickers))
                    .Where(x => x.Value.Count > 0)
                    .Where(x => x.Key != KnownTickers.TMON)
                    .ToDictionary();

                tickers = [.. candleData.Keys];

                for (int i = 0; i < tickers.Count; i++)
                {
                    for (int j = i + 1; j < tickers.Count; j++)
                    {
                        try
                        {
                            // Получаем свечи и синхронизируем свечи по дате                            
                            var (firstCandles, secondCandles) = StatArbitrageHelper.SyncCandles(
                                [.. candleData[tickers[i]].Where(x => x.Date >= from).Where(x => x.Date <= to)],
                                [.. candleData[tickers[j]].Where(x => x.Date >= from).Where(x => x.Date <= to)]
                                );

                            var firstData = PrepareData(firstCandles);
                            var secondData = PrepareData(secondCandles);

                            // Расчет корреляции
                            double correlationValue = firstData.Correlation(secondData);

                            if (Math.Abs(correlationValue) > statArbitrageSettings.CorrelationSettings.MinValue &&
                                Math.Abs(correlationValue) < statArbitrageSettings.CorrelationSettings.MaxValue)
                                await correlationRepository.AddAsync([
                                    new ()
                                    {
                                        PortfolioName = portfolioSetting.Name,
                                        TickerFirst = tickers[i],
                                        TickerSecond = tickers[j],
                                        Value = correlationValue
                                    }]);
                        }

                        catch (Exception exception)
                        {
                            logger.Error(exception, "Ошибка расчета корреляции. {tickerFirst}, {tickerSecond}", tickers[i], tickers[j]);
                        }
                    }
                }
            }

            return new();
        }

        private static List<double> PrepareData(List<Candle> candles)
        {
            var prices = candles.Select(x => x.Close).ToList();

            // Логарифмируем
            var logValues = prices.Log();

            // Центрируем
            var centeringValues = logValues.Centering();

            // Делим на стандартное отклонение
            var divStdValues = centeringValues.DivConst(centeringValues.StdDev());

            // Приращения
            var incrementValues = divStdValues.Increments(); 

            return incrementValues;
        }
    }
}
