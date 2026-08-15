using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Helpers
{
    public class PortfolioPositionHelper
    {
        /// <summary>
        /// Создать новую позицию
        /// </summary>
        /// <param name="targetWeight">Целевой вес в базовых юнитах</param>
        /// <param name="targetSize">Целевой размер в штуках</param>        
        /// <param name="currentPrice">Текущая цена</param>
        public static (PortfolioPosition Position, double MoneyChange) CreateNewPortfolioPosition(
            int targetWeight, int targetSize, double currentPrice)
        {
            var position = new PortfolioPosition
            {
                EntryPrice = currentPrice,
                Weight = targetWeight,
                Size = targetSize,
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
        /// <param name="targetSize">Целевой размер в штуках</param>
        /// <param name="currentPrice">Текущая цена</param>      
        public static (PortfolioPosition Position, double MoneyChange) UpLongPortfolioPosition(
            PortfolioPosition currentPosition, int targetWeight, int targetSize, double currentPrice)
        {
            int deltaSize = targetSize - currentPosition.Size;
            double entryPrice = currentPosition.EntryPrice!.Value;

            // Средняя цена открытия позиции
            double averageEntryPrice = (entryPrice * currentPosition.Size + currentPrice * deltaSize) / (currentPosition.Size + deltaSize);

            var position = new PortfolioPosition
            {
                EntryPrice = averageEntryPrice,
                Weight = targetWeight,
                Size = targetSize,
                Profit = (currentPrice - entryPrice) * currentPosition.Size
            };

            // Взять деньги на увеличение позиции
            double moneyChange = -1 * (currentPrice * deltaSize);

            return (position, moneyChange);
        }

        /// <summary>
        /// Нарастить короткую позицию
        /// </summary>
        /// <param name="currentPosition">Текущая открытая позиция</param>
        /// <param name="targetWeight">Целевой вес в базовых юнитах</param>
        /// <param name="targetSize">Целевой размер в штуках</param>
        /// <param name="currentPrice">Текущая цена</param>      
        public static (PortfolioPosition Position, double MoneyChange) UpShortPortfolioPosition(
            PortfolioPosition currentPosition, int targetWeight, int targetSize, double currentPrice)
        {
            int deltaSize = targetSize - currentPosition.Size;
            double entryPrice = currentPosition.EntryPrice!.Value;

            // Средняя цена открытия позиции
            double averageEntryPrice = (entryPrice * currentPosition.Size + currentPrice * deltaSize) / (currentPosition.Size + deltaSize);

            var position = new PortfolioPosition
            {
                EntryPrice = averageEntryPrice,
                Weight = targetWeight,
                Size = targetSize,
                Profit = (entryPrice - currentPrice) * currentPosition.Size
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
        /// <param name="targetSize">Целевой размер в штуках</param>
        /// <param name="currentPrice">Текущая цена</param>
        public static (PortfolioPosition Position, double MoneyChange) DownLongPortfolioPosition(
            PortfolioPosition currentPosition, int targetWeight, int targetSize, double currentPrice)
        {
            int deltaSize = currentPosition.Size - targetSize;
            double entryPrice = currentPosition.EntryPrice!.Value;

            var position = new PortfolioPosition
            {
                EntryPrice = entryPrice,
                Weight = targetWeight,
                Size = targetSize,
                Profit = (currentPrice - entryPrice) * targetSize
            };

            // Положить деньги от сокращения позиции
            double moneyChange = currentPrice * deltaSize;

            return (position, moneyChange);
        }

        /// <summary>
        /// Сократить короткую позицию
        /// </summary>
        /// <param name="currentPosition">Текущая открытая позиция</param>
        /// <param name="targetWeight">Целевой вес в базовых юнитах</param>
        /// <param name="targetSize">Целевой размер в штуках</param>
        /// <param name="currentPrice">Текущая цена</param>
        public static (PortfolioPosition Position, double MoneyChange) DownShortPortfolioPosition(
            PortfolioPosition currentPosition, int targetWeight, int targetSize, double currentPrice)
        {
            int deltaSize = currentPosition.Size - targetSize;
            double entryPrice = currentPosition.EntryPrice!.Value;

            var position = new PortfolioPosition
            {
                EntryPrice = entryPrice,
                Weight = targetWeight,
                Size = targetSize,
                Profit = (entryPrice - currentPrice) * targetSize
            };

            // Положить деньги от сокращения позиции
            double moneyChange = currentPrice * deltaSize;

            return (position, moneyChange);
        }

        /// <summary>
        /// Перевернуть длинную позицию
        /// </summary>
        /// <param name="currentPosition">Текущая открытая позиция</param>
        /// <param name="targetWeight">Целевой вес в базовых юнитах</param>
        /// <param name="targetSize">Целевой размер в штуках</param>
        /// <param name="currentPrice">Текущая цена</param>
        public static (PortfolioPosition Position, double MoneyChange) ReverseLongPortfolioPosition(
            PortfolioPosition currentPosition, int targetWeight, int targetSize, double currentPrice)
        {
            // Закрываем длинную позицию
            double moneyChangeCloseLong = currentPosition.EntryPrice!.Value * currentPosition.Size + (currentPrice - currentPosition.EntryPrice!.Value) * currentPosition.Size;

            // Открываем короткую позицию
            var (position, moneyChangeOpenShort) = CreateNewPortfolioPosition(targetWeight, targetSize, currentPrice);

            // Положить деньги от закрытия длинной позиции позиции и взять деньги на открытие короткой позиции
            double moneyChange = moneyChangeCloseLong + moneyChangeOpenShort; 

            return (position, moneyChange);
        }

        /// <summary>
        /// Перевернуть короткую позицию
        /// </summary>
        /// <param name="currentPosition">Текущая открытая позиция</param>
        /// <param name="targetWeight">Целевой вес в базовых юнитах</param>
        /// <param name="targetSize">Целевой размер в штуках</param>
        /// <param name="currentPrice">Текущая цена</param>
        public static (PortfolioPosition Position, double MoneyChange) ReverseShortPortfolioPosition(
            PortfolioPosition currentPosition, int targetWeight, int targetSize, double currentPrice)
        {
            // Закрываем короткую позицию
            double moneyChangeCloseShort = currentPosition.EntryPrice!.Value * currentPosition.Size + (currentPosition.EntryPrice!.Value - currentPrice) * currentPosition.Size;

            // Открываем длинную позицию
            var (position, moneyChangeOpenLong) = CreateNewPortfolioPosition(targetWeight, targetSize, currentPrice);

            // Положить деньги от закрытия короткой позиции позиции и взять деньги на открытие длинной позиции
            double moneyChange = moneyChangeCloseShort + moneyChangeOpenLong;

            return (position, moneyChange);
        }

        /// <summary>
        /// Закрыть длинную позицию
        /// </summary>
        /// <param name="currentPosition">Текущая открытая позиция</param>
        /// <param name="currentPrice">Текущая цена</param>
        public static (PortfolioPosition Position, double MoneyChange) CloseLongPortfolioPosition(
            PortfolioPosition currentPosition, double currentPrice)
        {
            // Закрываем длинную позицию       
            double moneyChangeCloseLong = currentPosition.EntryPrice!.Value * currentPosition.Size + (currentPrice - currentPosition.EntryPrice!.Value) * currentPosition.Size;
            var position = new PortfolioPosition();

            return (position, moneyChangeCloseLong);
        }

        /// <summary>
        /// Закрыть короткую позицию
        /// </summary>
        /// <param name="currentPosition">Текущая открытая позиция</param>
        /// <param name="currentPrice">Текущая цена</param>
        public static (PortfolioPosition Position, double MoneyChange) CloseShortPortfolioPosition(
            PortfolioPosition currentPosition, double currentPrice)
        {
            // Закрываем короткую позицию       
            double moneyChangeCloseShort = currentPosition.EntryPrice!.Value * currentPosition.Size + (currentPosition.EntryPrice!.Value - currentPrice) * currentPosition.Size;
            var position = new PortfolioPosition();

            return (position, moneyChangeCloseShort);
        }
    }
}
