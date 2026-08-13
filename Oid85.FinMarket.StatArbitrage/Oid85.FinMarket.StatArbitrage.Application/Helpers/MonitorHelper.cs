using Oid85.FinMarket.StatArbitrage.Common.Utils;
using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Helpers
{
    public class MonitorHelper
    {
        public static List<(string Ticker, List<DateWeight> WeightData)> GetPositionWeightData(
            List<StrategyExecuteResult> strategyExecuteResults,
            List<string> tickers,
            List<DateOnly> dates)
        {
            var result = new List<(string Ticker, List<DateWeight> WeightData)>();

            List<(string Ticker, List<List<DateValue<int>>> DateValueLists)> data = [];

            foreach (var ticker in tickers) data.Add((ticker, [.. strategyExecuteResults.Where(x => x.TickerFirst == ticker).Select(x => MapFirst(x.Positions, dates))]));
            foreach (var ticker in tickers) data.Add((ticker, [.. strategyExecuteResults.Where(x => x.TickerSecond == ticker).Select(x => MapSecond(x.Positions, dates))]));

            foreach (var ticker in tickers)
            {
                List<List<DateValue<int>>> tickerData = [];

                foreach (var item in data.Where(x => x.Ticker == ticker)) tickerData.AddRange(item.DateValueLists);

                result.Add(new(ticker, [.. Merge(tickerData, dates).Select(x => new DateWeight { Date = x.Date, Weight = x.Value })]));
            }

            return [.. result.OrderBy(x => x.Ticker)];
        }

        public static List<TickerWeight> GetPositionWeightDataByDate(
            List<(string Ticker, List<DateWeight> WeightData)> weightData,
            DateOnly date)
        {
            var result = new List<TickerWeight>();

            foreach (var (ticker, weight) in weightData)
                result.Add(
                    new()
                    {
                        Ticker = ticker,
                        Weight = weight.Find(x => x.Date == date)?.Weight ?? 0
                    });

            return result;
        }

        public static List<DateValue<int>> Merge(List<List<DateValue<int>>> data, List<DateOnly> dates)
        {
            var result = new List<DateValue<int>>();

            var combineData = data.SelectMany(x => x).ToList();

            foreach (var date in dates)
                result.Add(
                    new()
                    {
                        Date = date,
                        Value = combineData.Where(x => x.Date == date).Sum(x => x.Value)
                    });

            return [.. result.OrderBy(x => x.Date)];
        }

        public static List<DateValue<int>> MapFirst(SortedDictionary<DateOnly, Position> positions, List<DateOnly> dates) => 
            Map(positions, dates).First;

        public static List<DateValue<int>> MapSecond(SortedDictionary<DateOnly, Position> positions, List<DateOnly> dates) =>
            Map(positions, dates).Second;

        public static (List<DateValue<int>> First, List<DateValue<int>> Second) Map(SortedDictionary<DateOnly, Position> positions, List<DateOnly> dates)
        {
            var dictionaryFirst = dates.ToDictionary(k => k, v => 0);
            var dictionarySecond = dates.ToDictionary(k => k, v => 0);

            foreach (var position in positions)
            {
                var positionDates = position.Value.ExitDate.HasValue
                    ? DateUtils.GetDates(position.Value.EntryDate, position.Value.ExitDate.Value)
                    : DateUtils.GetDates(position.Value.EntryDate, dates.Last());

                foreach (var date in positionDates)
                {
                    if (position.Value.IsLongShort) { dictionaryFirst[date] = 1; dictionarySecond[date] = -1; }
                    if (position.Value.IsShortLong) { dictionaryFirst[date] = -1; dictionarySecond[date] = 1; }
                }
            }

            return (
                [.. dictionaryFirst.Select(x => new DateValue<int> { Date = x.Key, Value = x.Value }).OrderBy(x => x.Date)],
                [.. dictionarySecond.Select(x => new DateValue<int> { Date = x.Key, Value = x.Value }).OrderBy(x => x.Date)]);
        }
    }
}
