using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Core;
using Oid85.FinMarket.StatArbitrage.Core.Responses;
using Oid85.FinMarket.StatArbitrage.WebHost.Controller.Base;

namespace Oid85.FinMarket.StatArbitrage.WebHost.Controller;

/// <summary>
/// Бектест
/// </summary>
[Route("api/backtest")]
[ApiController]
public class BacktestController(
    IStatArbitrageService statArbitrageService)
    : BaseController
{
    /// <summary>
    /// Бэктест всех портфелей
    /// </summary>
    [HttpPost("portfolio")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestAsync() =>
        GetResponseAsync(
            () => statArbitrageService.BacktestAsync(new() { PortfolioName = string.Empty }),
            result => new BaseResponse<BacktestResponse> { Result = result });
}