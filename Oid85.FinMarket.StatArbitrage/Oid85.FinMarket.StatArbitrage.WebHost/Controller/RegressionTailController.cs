using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Core;
using Oid85.FinMarket.StatArbitrage.Core.Requests;
using Oid85.FinMarket.StatArbitrage.Core.Responses;
using Oid85.FinMarket.StatArbitrage.WebHost.Controller.Base;

namespace Oid85.FinMarket.StatArbitrage.WebHost.Controller;

/// <summary>
/// Хвосты регрессии
/// </summary>
[Route("api/regression-tail")]
[ApiController]
public class RegressionTailController(
    IRegressionTailService regressionTailService)
    : BaseController
{
    /// <summary>
    /// Расчитать хвосты регрессии
    /// </summary>
    [HttpPost("portfolio/calculate")]
    [ProducesResponseType(typeof(BaseResponse<CalculateRegressionTailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CalculateRegressionTailResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<CalculateRegressionTailResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> CalculateRegressionTailAsync(
        [FromBody] CalculateRegressionTailRequest request) =>
        GetResponseAsync(
            () => regressionTailService.CalculateRegressionTailAsync(request),
            result => new BaseResponse<CalculateRegressionTailResponse> { Result = result });
}