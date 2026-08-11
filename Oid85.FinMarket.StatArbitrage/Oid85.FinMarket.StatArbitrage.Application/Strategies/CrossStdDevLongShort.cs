using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Strategies;

public class CrossStdDevLongShort : Strategy
{
    public override void Execute()
    {
        // Получаем параметры
        double stdDev = Parameters["StdDev"] / 10.0;

        for (int i = StabilizationPeriod; i < Candles.First.Count - 1; i++)
        {
            var date = Candles.First[i].Date;
            var tail = Tails.Find(x => x.Date == date);

            if (tail is null)
                continue;

            // Правило входа
            SignalLongShort = tail.Value > 0.0;
            SignalLongShort &= Math.Abs(tail.Value) >= stdDev;

            // Правило выхода
            SignalCloseLongShort = tail.Value <= 0.0;
                
            // Задаем цену для заявки
            var orderPrice = (Candles.First[i].Close, Candles.Second[i].Close);

            // Расчет размера позиции
            var positionSize = GetPositionSize(orderPrice);
                
            if (LastActivePosition is null)
            {
                if (SignalLongShort)
                    BuySellAtPrice(positionSize, orderPrice, i + 1);
            }
                
            else
            {
                if (SignalCloseLongShort)
                    SellBuyAtPrice(positionSize, orderPrice, i + 1);
            }
                
            // Отрисовка
            DiagramPoints[i].PriceFirst = Candles.First[i].Close;
            DiagramPoints[i].PriceSecond = Candles.Second[i].Close;
        }
    }
}