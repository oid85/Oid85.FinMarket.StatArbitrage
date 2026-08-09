using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Core;
using Oid85.FinMarket.StatArbitrage.Core.Requests;
using Oid85.FinMarket.StatArbitrage.Core.Responses;
using Oid85.FinMarket.StatArbitrage.WebHost.Controller.Base;

namespace Oid85.FinMarket.StatArbitrage.WebHost.Controller;

/// <summary>
/// Корреляция
/// </summary>
[Route("api/correlation")]
[ApiController]
public class CorrelationController(
    ICorrelationService correlationService)
    : BaseController
{
    /// <summary>
    /// Расчитать корреляции
    /// </summary>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(BaseResponse<CalculateCorrelationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CalculateCorrelationResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<CalculateCorrelationResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> CalculateCorrelationAsync(
        [FromBody] CalculateCorrelationRequest request) =>
        GetResponseAsync(
            () => correlationService.CalculateCorrelationAsync(request),
            result => new BaseResponse<CalculateCorrelationResponse> { Result = result });
}