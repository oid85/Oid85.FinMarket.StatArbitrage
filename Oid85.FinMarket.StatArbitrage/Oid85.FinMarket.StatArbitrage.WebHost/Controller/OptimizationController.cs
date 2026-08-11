using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Core;
using Oid85.FinMarket.StatArbitrage.Core.Responses;
using Oid85.FinMarket.StatArbitrage.WebHost.Controller.Base;

namespace Oid85.FinMarket.StatArbitrage.WebHost.Controller;

/// <summary>
/// Оптимизация
/// </summary>
[Route("api/optimization")]
[ApiController]
public class OptimizationController(
    IStatArbitrageService statArbitrageService)
    : BaseController
{
    /// <summary>
    /// Оптимизация всех портфелей
    /// </summary>
    [HttpPost("portfolio")]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<OptimizationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> OptimizationAsync() =>
        GetResponseAsync(
            () => statArbitrageService.OptimizationAsync(new() { PortfolioName = string.Empty }),
            result => new BaseResponse<OptimizationResponse> { Result = result });
}