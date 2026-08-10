using Accord.Statistics.Models.Regression.Linear;
using Microsoft.Extensions.Options;
using NLog;
using Oid85.FinMarket.StatArbitrage.Application.Helpers;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.ApiClients;
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
    public class RegressionTailService(
        ILogger logger,
        IOptions<StatArbitrageSettings> options,
        IDataService dataService,
        ICorrelationRepository correlationRepository,
        IRegressionTailRepository regressionTailRepository,
        IComputationApiClient computationApiClient) 
        : IRegressionTailService
    {
        public async Task<CalculateRegressionTailResponse> CalculateRegressionTailAsync(CalculateRegressionTailRequest request)
        {
            var statArbitrageSettings = options.Value;
            var portfolioSettingsList = statArbitrageSettings.Portfolios;

            if (!string.IsNullOrEmpty(request.PortfolioName))
                portfolioSettingsList = [.. portfolioSettingsList.Where(x => x.Name == request.PortfolioName)];

            var (from, to) = StatArbitrageHelper.GetOptimizationDates(options.Value);

            foreach (var portfolioSetting in portfolioSettingsList)
            {
                await regressionTailRepository.DeleteAsync(portfolioSetting.Name);

                var correlations = await correlationRepository.GetAsync(portfolioSetting.Name);

                List<string> tickers = [
                    ..correlations.Select(x => x.TickerFirst),
                    ..correlations.Select(x => x.TickerSecond)
                    ];

                var candleData = (await dataService.GetCandleDataAsync([.. tickers.Distinct()]))
                    .Where(x => x.Value.Count > 0)
                    .Where(x => x.Key != KnownTickers.TMON)
                    .ToDictionary();

                tickers = [.. candleData.Keys];

                foreach (var correlation in correlations)
                {
                    try
                    {
                        // Получаем свечи и синхронизируем свечи по дате                            
                        var (firstCandles, secondCandles) = StatArbitrageHelper.SyncCandles(
                            [.. candleData[correlation.TickerFirst].Where(x => x.Date >= from).Where(x => x.Date <= to)],
                            [.. candleData[correlation.TickerSecond].Where(x => x.Date >= from).Where(x => x.Date <= to)]
                            );

                        // Declare some sample test data.
                        double[] inputs = [.. secondCandles.Select(x => x.Close)];
                        double[] outputs = [.. firstCandles.Select(x => x.Close)];

                        // Use Ordinary Least Squares to learn the regression
                        var ols = new OrdinaryLeastSquares();

                        // Use OLS to learn the simple linear regression
                        SimpleLinearRegression regression = ols.Learn(inputs, outputs);

                        // We can also extract the slope and the intercept term for the line
                        double slope = regression.Slope;
                        double intercept = regression.Intercept;

                        // Расчет хвостов
                        var regressionTailSet = new RegressionTailSet
                        {
                            PortfolioName = portfolioSetting.Name,
                            TickerFirst = correlation.TickerFirst,
                            TickerSecond = correlation.TickerSecond
                        };

                        for (int i = 0; i < firstCandles.Count; i++)
                        {
                            double y = slope * secondCandles[i].Close + intercept;
                            double tailValue = firstCandles[i].Close - y;

                            regressionTailSet.Tails.Add(
                                new ()
                                { 
                                    Date = firstCandles[i].Date,
                                    Value = tailValue
                                });
                        }

                        regressionTailSet.Slope = slope;
                        regressionTailSet.Intercept = intercept;

                        // Расчитаем Z-score
                        regressionTailSet.Tails = ZScore(regressionTailSet.Tails);

                        // Проверяем на стационарность и сохраняем
                        var checkStationaryResult = await computationApiClient.CheckStationaryAsync([[.. regressionTailSet.Tails.Select(x => x.Value)]]);

                        if (checkStationaryResult[0])
                            await regressionTailRepository.AddAsync([regressionTailSet]);
                    }

                    catch (Exception exception)
                    {
                        logger.Error(exception, "Ошибка расчета остатков регрессии. {tickerFirst}, {tickerSecond}", correlation.TickerFirst, correlation.TickerSecond);
                    }
                }
            }

            return new();
        }

        public async Task<GetRegressionTailResponse> GetRegressionTailAsync(GetRegressionTailRequest request)
        {
            await CalculateRegressionTailAsync(new () { PortfolioName = request.PortfolioName });

            var statArbitrageSettings = options.Value;

            if (string.IsNullOrEmpty(request.PortfolioName))
                request.PortfolioName = statArbitrageSettings.Portfolios.First().Name;

            var from = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * 15));
            var to = DateOnly.FromDateTime(DateTime.Today);

            var dates = DateUtils.GetDates(from, to);

            var regressionTailSet = (await regressionTailRepository.GetAsync(request.PortfolioName));

            var response = new GetRegressionTailResponse
            { 
                PortfolioName = request.PortfolioName,
                Dates = dates,
                Items = [.. regressionTailSet
                    .Select(x => 
                    new RegressionTailData
                    {
                        TickerFirst = x.TickerFirst,
                        TickerSecond = x.TickerSecond,
                        Tails = [.. dates
                            .Select(date =>
                            {
                                double? value = x.Tails.Find(dateValue => dateValue.Date == date)?.Value;
                                string colorFill = GetColor(value);

                                return
                                new RegressionTailDataItem()
                                {
                                    Date = date,
                                    Value = value.RoundTo(2),
                                    ColorFill = colorFill
                                };

                                static string GetColor(double? value)
                                {
                                    if (!value.HasValue)
                                        return KnownColors.White;

                                    return value.Value switch 
                                    { 
                                        >= 3.0 => KnownColors.DarkGreen, 
                                        >= 2.0 => KnownColors.Green, 
                                        >= 1.0 => KnownColors.LightGreen, 
                                        >= -1.0 => KnownColors.White, 
                                        >= -2.0 => KnownColors.LightRed, 
                                        >= -3.0 => KnownColors.Red, 
                                        _ => KnownColors.DarkRed
                                    };
                                }
                            })]
                    })]
            };

            return response;
        }

        /// <summary>
        /// Z-score
        /// </summary>
        private static List<DateValue<double>> ZScore(List<DateValue<double>> values)
        {
            if (values.Count == 0)
                return [];

            var dates = values.Select(x => x.Date).ToList();
            var tailValues = values.Select(x => x.Value).ToList();

            var average = tailValues.Average();
            var stdDev = tailValues.StdDev();

            if (stdDev == 0.0)
                return [];

            var zScoreValues = tailValues.AddConst(-1 * average).DivConst(stdDev);

            var result = new List<DateValue<double>>();

            for (int i = 0; i < dates.Count; i++)
                result.Add(
                    new DateValue<double> 
                    { 
                        Date = dates[i], 
                        Value = zScoreValues[i] 
                    });

            return result;
        }
    }
}
