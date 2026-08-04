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
    /// Сгенерировать печатную форму
    /// </summary>
    [HttpPost("monitor")]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> MonitorAsync(
        [FromBody] MonitorRequest request) =>
        GetResponseAsync(
            () => statArbitrageService.MonitorAsync(request),
            result => new BaseResponse<MonitorResponse> { Result = result });
}