using Oid85.PrintTemplate.Core.Requests;
using Oid85.PrintTemplate.Core.Responses;

namespace Oid85.PrintTemplate.Application.Interfaces.Services
{
    public interface IPrintTemplateService
    {
        Task<CreatePrintTemplateResponse> CreatePrintTemplateAsync(CreatePrintTemplateRequest request);
    }
}
