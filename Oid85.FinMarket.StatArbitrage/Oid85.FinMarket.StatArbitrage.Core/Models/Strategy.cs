namespace Oid85.FinMarket.StatArbitrage.Core.Models;

public class Strategy
{
    public double StartMoney { get; set; }

    public double EndMoney { get; set; }

    public (string First, string Second) Ticker { get; set; } = (string.Empty, string.Empty);

    public (bool First, bool Second) IsFuture { get; set; } = (false, false);

    public (double First, double Second) BasicAssetSize { get; set; } = (1.0, 1.0);

    public (double First, double Second) Leverage { get; set; } = (1.0, 1.0);

    public string StrategyDescription { get; set; } = string.Empty;
    
    public string StrategyName { get; set; } = string.Empty;

    public string PortfolioName { get; set; } = string.Empty;

    public string ProcessName { get; set; } = string.Empty;

    public DateOnly StartDate => Candles.First.First().Date;
    
    public DateOnly EndDate => Candles.First.Last().Date;
    
    public Dictionary<string, int> Parameters { get; set; } = [];

    public int StabilizationPeriod { get; set; } = 1;
    
    public Dictionary<string, List<Candle>> CandleData { get; set; } = [];

    public (List<Candle> First, List<Candle> Second) Candles { get; set; } = ([], []);

    public (List<double> First, List<double> Second) OpenPrices => (Candles.First.Select(x => x.Open).ToList(), Candles.Second.Select(x => x.Open).ToList());

    public (List<double> First, List<double> Second) ClosePrices => (Candles.First.Select(x => x.Close).ToList(), Candles.Second.Select(x => x.Close).ToList());

    public (List<double> First, List<double> Second) HighPrices => (Candles.First.Select(x => x.High).ToList(), Candles.Second.Select(x => x.High).ToList());

    public (List<double> First, List<double> Second) LowPrices => (Candles.First.Select(x => x.Low).ToList(), Candles.Second.Select(x => x.Low).ToList());

    public List<DateValue<double>> Spreads { get; set; } = [];

    public List<DiagramPoint> DiagramPoints { get; set; } = [];
    
    public bool SignalLongShort { get; set; }
    
    public bool SignalShortLong { get; set; }

    public bool FilterLongShort { get; set; } = true;

    public bool FilterShortLong { get; set; } = true;
    
    public bool SignalCloseLongShort { get; set; }
    
    public bool SignalCloseShortLong { get; set; }

    public SortedDictionary<DateOnly, Position> Positions { get; set; } = [];

    public (int First, int Second) GetPositionSize((double First, double Second) orderPrice)
    {
        if (EndMoney <= orderPrice.First + orderPrice.Second)
            return (0, 0);

        double money = EndMoney / 2.0;

        (int First, int Second) positionSize = (0, 0);

        if (IsFuture.First)
            positionSize.First = orderPrice.First == 0.0 || BasicAssetSize.First == 0.0 ? 0 : Convert.ToInt32(money / (orderPrice.First * BasicAssetSize.First) * Leverage.First);

        else
            positionSize.First = orderPrice.First == 0.0 ? 0 : Convert.ToInt32(money / orderPrice.First * Leverage.First);

        if (IsFuture.Second)
            positionSize.Second = orderPrice.Second == 0.0 || BasicAssetSize.Second == 0.0 ? 0 : Convert.ToInt32(money / (orderPrice.Second * BasicAssetSize.Second) * Leverage.Second);

        else
            positionSize.Second = orderPrice.Second == 0.0 ? 0 : Convert.ToInt32(money / orderPrice.Second * Leverage.Second);

        if (positionSize.First == 0 || positionSize.Second == 0)
            return (0, 0);

        return positionSize;
    }

    public Position? LastActivePosition {
        get
        {
            if (LastPosition is null)
                return null;
            
            if (LastPosition.IsActive)
                return LastPosition;

            return null;
        }
    }

    public Position? LastPosition => Positions.Count == 0 ? null : Positions.Last().Value;

    public (int First, int Second) CurrentPosition
    {
        get
        {
            if (LastActivePosition == null)
                return (0, 0);

            if (LastActivePosition.IsLongShort)
                return (Math.Abs(LastActivePosition.Quantity.First), -1 * Math.Abs(LastActivePosition.Quantity.Second));

            if (LastActivePosition.IsShortLong)
                return (-1 * Math.Abs(LastActivePosition.Quantity.First), Math.Abs(LastActivePosition.Quantity.Second));

            return (0, 0);
        }
    }

    public double CurrentPositionCost
    {
        get
        {
            if (LastActivePosition == null)
                return 0.0;

            if (LastActivePosition.IsLongShort)
                return LastActivePosition.Cost;

            if (LastActivePosition.IsShortLong)
                return -1 * Math.Abs(LastActivePosition.Cost);

            return 0.0;
        }
    }

    public void BuySellAtPrice((int First, int Second) quantity, (double First, double Second) price, int candleIndex) =>
        AddTrade(new Trade
        {
            CandleIndex = candleIndex,
            Quantity = (Math.Abs(quantity.First), -1 * Math.Abs(quantity.Second)),
            Price = (price.First, price.Second),
            Date = Candles.First[candleIndex].Date
        });

    public void SellBuyAtPrice((int First, int Second) quantity, (double First, double Second) price, int candleIndex) =>
        AddTrade(new Trade
        {
            CandleIndex = candleIndex,
            Quantity = (-1 * Math.Abs(quantity.First), Math.Abs(quantity.Second)),
            Price = (price.First, price.Second),
            Date = Candles.First[candleIndex].Date
        });

    private void AddTrade(Trade trade)
    {
        if (trade.Quantity.First == 0 || trade.Quantity.Second == 0)
            return;

        if (LastActivePosition is null)
            Positions.Add(
                trade.Date, 
                new()
                {
                    Ticker = Ticker,
                    EntryPrice = trade.Price,
                    EntryDate = trade.Date,
                    EntryCandleIndex = trade.CandleIndex,
                    IsActive = true,
                    IsLongShort = trade.Quantity is { First: > 0, Second: < 0 },
                    IsShortLong = trade.Quantity is { First: < 0, Second: > 0 },
                    Quantity = (trade.Quantity.First, trade.Quantity.Second),
                    Cost = Math.Abs(trade.Quantity.First) * trade.Price.First + Math.Abs(trade.Quantity.Second) * trade.Price.Second
                });

        else
        {
            int count = Positions.Count;

            var key = Positions.Last().Key;

            Positions[key].ExitPrice = trade.Price;
            Positions[key].ExitDate = trade.Date;
            Positions[key].ExitCandleIndex = trade.CandleIndex;
            Positions[key].IsActive = false;

            double profit = 0.0;

            if (Positions[key].IsLongShort)
            {
                profit += Math.Abs(Positions[key].Quantity.First) * (Positions[key].ExitPrice.First!.Value - Positions[key].EntryPrice.First);
                profit += Math.Abs(Positions[key].Quantity.Second) * (Positions[key].EntryPrice.Second - Positions[key].ExitPrice.Second!.Value);
            }

            if (Positions[key].IsShortLong)
            {
                profit += Math.Abs(Positions[key].Quantity.First) * (Positions[key].EntryPrice.First - Positions[key].ExitPrice.First!.Value);
                profit += Math.Abs(Positions[key].Quantity.Second) * (Positions[key].ExitPrice.Second!.Value - Positions[key].EntryPrice.Second);
            }

            Positions[key].NetProfit = profit;
            Positions[key].NetProfitPercent = profit / EndMoney * 100.0;
            
            var totalProfit = Positions.Sum(x => x.Value.NetProfit);
            Positions[key].TotalNetProfit = totalProfit;
            Positions[key].TotalProfitPct = totalProfit / EndMoney * 100.0;
            
            EndMoney += profit;
            
            EqiutyCurve.TryAdd(Positions[key].ExitDate!.Value, Positions[key].TotalNetProfit);
            
            double drawdown;
            
            if (count < 2)
                drawdown = 0.0;

            else
            {
                var maxTotalProfit = Positions.Take(count - 1).Max(x => x.Value.TotalNetProfit);

                drawdown = Positions[key].TotalNetProfit >= maxTotalProfit
                    ? 0.0
                    : maxTotalProfit - Positions[key].TotalNetProfit;
            }
            
            DrawdownCurve.TryAdd(Positions[key].ExitDate!.Value, drawdown);
        }
    }

    public void CloseAtPrice(Position position, (double First, double Second) price, int candleIndex)
    {
        // Отправляем команды, если длинная позиция
        if (position.IsLongShort)
            SellBuyAtPrice((position.Quantity.First, position.Quantity.Second), (price.First, price.Second), candleIndex);

        // Отправляем команды, если короткая позиция
        else if (position.IsShortLong)
            BuySellAtPrice((position.Quantity.First, position.Quantity.Second), (price.First, price.Second), candleIndex);
    }

    public Dictionary<DateOnly, double> EqiutyCurve { get; set; } = [];

    public Dictionary<DateOnly, double> DrawdownCurve  { get; set; } = [];
    
    public double ProfitFactor
    {
        get
        {
            if (Positions.Count == 0) return 0.0;

            double profits = Positions.Where(x => x.Value.NetProfit > 0.0).Sum(x => x.Value.NetProfit);
            double losses = Positions.Where(x => x.Value.NetProfit < 0.0).Sum(x => x.Value.NetProfit);

            if (losses == 0.0) return double.PositiveInfinity;
            
            return profits / Math.Abs(losses);
        }
    }

    public double RecoveryFactor => MaxDrawdown == 0.0 ? double.PositiveInfinity : TotalNetProfit / MaxDrawdown;

    public double TotalNetProfit => Positions.Count == 0 ? 0.0 : Positions.Sum(x => x.Value.NetProfit);

    public double AverageNetProfit => Positions.Count == 0 ? 0.0 : Positions.Select(x => x.Value.NetProfit).Average();

    public double AverageNetProfitPercent => Positions.Count == 0 ? 0.0 : Positions.Select(x => x.Value.NetProfitPercent).Average();

    public double Drawdown  => LastPosition is null ? 0.0 : Positions.Max(x => x.Value.TotalNetProfit) - LastPosition.TotalNetProfit;

    public double MaxDrawdown  => DrawdownCurve.Count == 0 ? 0.0 : Math.Abs(DrawdownCurve.Max(x => x.Value));

    public double MaxDrawdownPercent => EqiutyCurve.Count == 0 ? 0.0 : Math.Abs(MaxDrawdown / EqiutyCurve.Max(x => x.Value) * 100.0);

    public int NumberPositions => Positions.Count;

    public int WinningPositions => Positions.Count == 0 ? 0 : Positions.Count(x => x.Value.NetProfit > 0.0);

    public double WinningTradesPercent => NumberPositions == 0.0 ? 0.0 : Convert.ToDouble(WinningPositions) / Convert.ToDouble(NumberPositions) * 100.0;    
    
    public double TotalReturn => EndMoney > StartMoney ? (EndMoney - StartMoney) / StartMoney * 100.0 : 0.0;
    
    public double AnnualYieldReturn => EndMoney > StartMoney ? TotalReturn / ((EndDate.DayNumber - StartDate.DayNumber) / 365.0): 0.0;
    
    public virtual void Execute()
    {

    }

    public void Init(Dictionary<string, int> parameterSet, double money)
    {
        Parameters = parameterSet;
        Positions.Clear();
        EqiutyCurve.Clear();
        DrawdownCurve.Clear();
        StartMoney = money;
        EndMoney = money;

        DiagramPoints.Clear();
        for (int i = 0; i < Candles.First.Count; i++)
            DiagramPoints.Add(new()
            {
                Index = Candles.First[i].Index,
                Date = Candles.First[i].Date
            });
    }
}