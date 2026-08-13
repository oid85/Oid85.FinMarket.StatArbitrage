using System.Timers;
using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Helpers
{
    public class PortfolioPositionHelper
    {
        /// <summary>
        /// Создать новую позицию
        /// </summary>
        /// <param name="targetWeight">Целевой вес в базовых юнитах</param>
        /// <param name="currentPrice">Текущая цена</param>
        /// <param name="targetSize">Целевой размер в штуках</param>        
        public static (PortfolioPosition Position, double MoneyChange) CreateNewPortfolioPosition(
            int targetWeight, double currentPrice, int targetSize)
        {
            var position = new PortfolioPosition
            {
                IsActive = true,
                IsLong = targetWeight > 0,
                IsShort = targetWeight < 0,
                EntryPrice = currentPrice,
                Weight = targetWeight,
                Size = targetSize,
                Cost = currentPrice * targetSize,
                Profit = 0.0
            };

            // Взять деньги на открытие позиции
            double moneyChange = -1 * (currentPrice * targetSize);

            return (position, moneyChange);
        }

        /// <summary>
        /// Нарастить длинную позицию
        /// </summary>
        /// <param name="currentPosition">Текущая открытая позиция</param>
        /// <param name="targetWeight">Целевой вес в базовых юнитах</param>
        /// <param name="currentPrice">Текущая цена</param>
        /// <param name="targetSize">Целевой размер в штуках</param>        
        public static (PortfolioPosition Position, double MoneyChange) UpLongPortfolioPosition(
            PortfolioPosition currentPosition, int targetWeight, double currentPrice, int targetSize)
        {
            int deltaSize = targetSize - currentPosition.Size;
            double entryPrice = currentPosition.EntryPrice!.Value;

            // Средняя цена открытия позиции
            double averageEntryPrice = (entryPrice + currentPrice * deltaSize) / (deltaSize + 1);

            var position = new PortfolioPosition
            {
                IsActive = true,
                IsLong = true,
                IsShort = false,
                EntryPrice = averageEntryPrice,
                Weight = targetWeight,
                Size = targetSize,
                Cost = currentPrice * targetSize,
                Profit = (currentPrice - averageEntryPrice) * targetSize
            };

            // Взять деньги на увеличение позиции
            double moneyChange = -1 * (currentPrice * deltaSize);

            return (position, moneyChange);
        }

        /// <summary>
        /// Сократить длинную позицию
        /// </summary>
        /// <param name="currentPosition">Текущая открытая позиция</param>
        /// <param name="targetWeight">Целевой вес в базовых юнитах</param>
        /// <param name="currentPrice">Текущая цена</param>
        /// <param name="targetSize">Целевой размер в штуках</param>        
        public static (PortfolioPosition Position, double MoneyChange) DownLongPortfolioPosition(
            PortfolioPosition currentPosition, int targetWeight, double currentPrice, int targetSize)
        {
            int deltaSize = currentPosition.Size - targetSize;
            double entryPrice = currentPosition.EntryPrice!.Value;

            var position = new PortfolioPosition
            {
                IsActive = true,
                IsLong = true,
                IsShort = false,
                EntryPrice = entryPrice,
                Weight = targetWeight,
                Size = targetSize,
                Cost = currentPrice * targetSize,
                Profit = (currentPrice - entryPrice) * targetSize
            };

            // Положить деньги от сокращения позиции
            double moneyChange = currentPrice * deltaSize;

            return (position, moneyChange);
        }
    }
}
