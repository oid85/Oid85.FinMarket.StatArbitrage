using Oid85.FinMarket.StatArbitrage.Core.Requests;
using Oid85.FinMarket.StatArbitrage.Core.Responses;

namespace Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services
{
    public interface IStatArbitrageService
    {
        /// <summary>
        /// Бэктест стратегий портфеля
        /// </summary>
        Task<BacktestResponse> BacktestAsync(BacktestRequest request);

        /// <summary>
        /// Оптимизация стратегий портфеля
        /// </summary>
        Task<OptimizationResponse> OptimizationAsync(OptimizationRequest request);

        /// <summary>
        /// Мониторинг стратегий
        /// </summary>
        Task<MonitorResponse> MonitorAsync(MonitorRequest request);
        
        /// <summary>
        /// Список портфелей
        /// </summary>
        Task<PortfolioListResponse> PortfolioListAsync(PortfolioListRequest request);

        /// <summary>
        /// Получить сумму портфеля
        /// </summary>
        Task<GetPortfolioTotalSumResponse> GetPortfolioTotalSumAsync(GetPortfolioTotalSumRequest request);

        /// <summary>
        /// Редактировать сумму портфеля
        /// </summary>
        Task<EditPortfolioTotalSumResponse> EditPortfolioTotalSumAsync(EditPortfolioTotalSumRequest request);
    }
}
