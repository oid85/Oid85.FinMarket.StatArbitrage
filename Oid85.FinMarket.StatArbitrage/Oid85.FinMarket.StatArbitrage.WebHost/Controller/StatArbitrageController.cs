using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Core;
using Oid85.FinMarket.StatArbitrage.Core.Requests;
using Oid85.FinMarket.StatArbitrage.Core.Responses;
using Oid85.FinMarket.StatArbitrage.WebHost.Controller.Base;

namespace Oid85.FinMarket.StatArbitrage.WebHost.Controller;

/// <summary>
/// Статистический арбитраж
/// </summary>
[Route("api/stat-arbitrage")]
[ApiController]
public class StatArbitrageController(
    IStatArbitrageService statArbitrageService)
    : BaseController
{
    /// <summary>
    /// Мониторинг
    /// </summary>
    [HttpPost("portfolio/monitor")]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> MonitorAsync(
        [FromBody] MonitorRequest request) =>
        GetResponseAsync(
            () => statArbitrageService.MonitorAsync(request),
            result => new BaseResponse<MonitorResponse> { Result = result });

    /// <summary>
    /// Список портфелей
    /// </summary>
    [HttpPost("portfolio/list")]
    [ProducesResponseType(typeof(BaseResponse<PortfolioListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioListResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<PortfolioListResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> PortfolioListAsync(
        [FromBody] PortfolioListRequest request) =>
        GetResponseAsync(
            () => statArbitrageService.PortfolioListAsync(request),
            result => new BaseResponse<PortfolioListResponse> { Result = result });

    /// <summary>
    /// Получить сумму портфеля
    /// </summary>
    [HttpPost("portfolio/total-sum/get")]
    [ProducesResponseType(typeof(BaseResponse<GetPortfolioTotalSumResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetPortfolioTotalSumResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<GetPortfolioTotalSumResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> GetPortfolioTotalSumAsync(
        [FromBody] GetPortfolioTotalSumRequest request) =>
        GetResponseAsync(
            () => statArbitrageService.GetPortfolioTotalSumAsync(request),
            result => new BaseResponse<GetPortfolioTotalSumResponse> { Result = result });

    /// <summary>
    /// Редактировать сумму портфеля
    /// </summary>
    [HttpPost("portfolio/total-sum/edit")]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioTotalSumResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioTotalSumResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioTotalSumResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> EditPortfolioTotalSumAsync(
        [FromBody] EditPortfolioTotalSumRequest request) =>
        GetResponseAsync(
            () => statArbitrageService.EditPortfolioTotalSumAsync(request),
            result => new BaseResponse<EditPortfolioTotalSumResponse> { Result = result });
}