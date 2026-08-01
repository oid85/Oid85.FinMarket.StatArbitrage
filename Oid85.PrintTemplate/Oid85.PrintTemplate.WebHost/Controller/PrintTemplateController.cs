using Microsoft.AspNetCore.Mvc;
using Oid85.PrintTemplate.Application.Interfaces.Services;
using Oid85.PrintTemplate.Core;
using Oid85.PrintTemplate.Core.Requests;
using Oid85.PrintTemplate.Core.Responses;
using Oid85.PrintTemplate.WebHost.Controller.Base;

namespace Oid85.PrintTemplate.WebHost.Controller;

/// <summary>
/// Печатные формы
/// </summary>
[Route("api/print-template")]
[ApiController]
public class PrintTemplateController(
    IPrintTemplateService printTemplateService)
    : BaseController
{
    /// <summary>
    /// Сгенерировать печатную форму
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(BaseResponse<CreatePrintTemplateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreatePrintTemplateResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<CreatePrintTemplateResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> CreatePrintTemplateAsync(
        [FromBody] CreatePrintTemplateRequest request) =>
        GetResponseAsync(
            () => printTemplateService.CreatePrintTemplateAsync(request),
            result => new BaseResponse<CreatePrintTemplateResponse> { Result = result });
}