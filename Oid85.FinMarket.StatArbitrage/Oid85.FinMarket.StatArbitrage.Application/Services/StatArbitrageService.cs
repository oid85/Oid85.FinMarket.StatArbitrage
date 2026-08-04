using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Core.Requests;
using Oid85.FinMarket.StatArbitrage.Core.Responses;

namespace Oid85.FinMarket.StatArbitrage.Application.Services
{
    public class StatArbitrageService : IStatArbitrageService
    {
        public async Task<MonitorResponse> MonitorAsync(MonitorRequest request)
        {
            return new ();
        }
    }
}
