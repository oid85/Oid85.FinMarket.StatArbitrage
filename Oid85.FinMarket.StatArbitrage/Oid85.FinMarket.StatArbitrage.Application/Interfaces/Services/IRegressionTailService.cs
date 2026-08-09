using Oid85.FinMarket.StatArbitrage.Core.Requests;
using Oid85.FinMarket.StatArbitrage.Core.Responses;

namespace Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services
{
    public interface IRegressionTailService
    {
        Task<CalculateRegressionTailResponse> CalculateRegressionTailAsync(CalculateRegressionTailRequest request);
    }
}
