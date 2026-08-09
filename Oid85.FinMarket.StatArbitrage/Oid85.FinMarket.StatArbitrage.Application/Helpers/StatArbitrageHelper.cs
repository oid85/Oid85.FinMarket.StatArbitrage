using Microsoft.Extensions.Options;
using Oid85.FinMarket.StatArbitrage.Core.Configuration;
using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Helpers
{
    public class StatArbitrageHelper
    {
        /// <summary>
        /// Синхронизация свечей
        /// </summary>
        public static (List<Candle> First, List<Candle> Second) SyncCandles(List<Candle> candles1, List<Candle> candles2)
        {
            var dates1 = candles1.Select(x => x.Date).ToList();
            var dates2 = candles2.Select(x => x.Date).ToList();

            var dates = dates1.Intersect(dates2).ToList();

            var resultCandles1 = candles1.Where(x => dates.Contains(x.Date)).OrderBy(x => x.Date).ToList();
            var resultCandles2 = candles2.Where(x => dates.Contains(x.Date)).OrderBy(x => x.Date).ToList();

            return (resultCandles1, resultCandles2);
        }

        /// <summary>
        /// Получить даты для оптимизации
        /// </summary>
        public static (DateOnly From, DateOnly To) GetOptimizationDates(StatArbitrageSettings statArbitrageSettings)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var from = today
                .AddDays(-1 * statArbitrageSettings.BacktestSettings.BacktestWindowInDays)
                .AddDays(-1 * statArbitrageSettings.BacktestSettings.StabilizationPeriodInCandles)
                .AddDays(-1 * statArbitrageSettings.BacktestSettings.BacktestShiftInDays);

            var to = today.AddDays(-1 * statArbitrageSettings.BacktestSettings.BacktestShiftInDays);

            return (from, to);
        }

        /// <summary>
        /// Получить даты для бэктеста
        /// </summary>
        public static (DateOnly From, DateOnly To) GetBacktestDates(StatArbitrageSettings statArbitrageSettings)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var from = today
                .AddDays(-1 * statArbitrageSettings.BacktestSettings.BacktestWindowInDays)
                .AddDays(-1 * statArbitrageSettings.BacktestSettings.StabilizationPeriodInCandles);

            var to = today;

            return (from, to);
        }
    }
}
