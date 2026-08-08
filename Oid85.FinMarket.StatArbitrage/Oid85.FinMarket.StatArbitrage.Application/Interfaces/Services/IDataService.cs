using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services
{
    public interface IDataService
    {
        Task<Dictionary<string, List<Candle>>> GetCandleDataAsync(List<string> tickers);
        Task<Dictionary<string, Instrument>> GetInstrumentDataAsync(List<string> tickers);
        double? GetPrice(string ticker, DateOnly date);
    }
}
